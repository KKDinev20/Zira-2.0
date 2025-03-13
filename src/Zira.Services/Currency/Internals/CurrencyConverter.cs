using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Zira.Services.Currency.Contracts;

namespace Zira.Services.Currency
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private const string CacheKey = "ExchangeRates";
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;
        private readonly Dictionary<string, Dictionary<string, decimal>> exchangeRates;
        private readonly TimeSpan cacheExpirationTime = TimeSpan.FromHours(24);

        public CurrencyConverter(
            IMemoryCache cache,
            IConfiguration configuration)
        {
            this.cache = cache;
            this.configuration = configuration;

            this.exchangeRates = this.InitializeExchangeRates();
        }

        public async Task<decimal> ConvertCurrencyAsync(
            Guid userId,
            decimal amount,
            string fromCurrency,
            string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
            {
                return amount;
            }

            if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return amount;
            }

            var rate = this.GetExchangeRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }

        public async Task<decimal> ConvertCurrencyAsync(Guid userId, decimal amount, string toCurrency)
        {
            return amount;
        }

        public IDictionary<string, string> GetAvailableCurrencies()
        {
            return new Dictionary<string, string>
            {
                { "USD", "US Dollar" },
                { "EUR", "Euro" },
                { "GBP", "British Pound" },
                { "JPY", "Japanese Yen" },
                { "CAD", "Canadian Dollar" },
                { "AUD", "Australian Dollar" },
                { "CHF", "Swiss Franc" },
                { "CNY", "Chinese Yuan" },
                { "BGN", "Bulgarian Lev" },
            };
        }

        public void UpdateExchangeRate(string fromCurrency, string toCurrency, decimal rate)
        {
            fromCurrency = fromCurrency.ToUpperInvariant();
            toCurrency = toCurrency.ToUpperInvariant();

            var rates = this.cache.Get<Dictionary<string, Dictionary<string, decimal>>>(CacheKey) ?? this.exchangeRates;

            if (!rates.ContainsKey(fromCurrency))
            {
                rates[fromCurrency] = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            }

            rates[fromCurrency][toCurrency] = rate;

            if (!rates.ContainsKey(toCurrency))
            {
                rates[toCurrency] = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            }

            if (rate != 0)
            {
                rates[toCurrency][fromCurrency] = 1 / rate;
            }

            this.cache.Set(CacheKey, rates, this.cacheExpirationTime);
        }

        private Dictionary<string, Dictionary<string, decimal>> LoadRatesFromConfiguration()
        {
            try
            {
                var configSection = this.configuration.GetSection("ExchangeRates");
                if (!configSection.Exists())
                {
                    return null;
                }

                var result = new Dictionary<string, Dictionary<string, decimal>>(StringComparer.OrdinalIgnoreCase);

                foreach (var fromCurrency in configSection.GetChildren())
                {
                    var fromCurrencyCode = fromCurrency.Key;
                    var toCurrencies = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                    foreach (var toCurrency in fromCurrency.GetChildren())
                    {
                        if (decimal.TryParse(toCurrency.Value, out decimal rate))
                        {
                            toCurrencies[toCurrency.Key] = rate;
                        }
                    }

                    if (toCurrencies.Count > 0)
                    {
                        result[fromCurrencyCode] = toCurrencies;
                    }
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        private decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            fromCurrency = fromCurrency.ToUpperInvariant();
            toCurrency = toCurrency.ToUpperInvariant();

            if (this.cache.TryGetValue(CacheKey, out Dictionary<string, Dictionary<string, decimal>> cachedRates))
            {
                if (this.TryGetRateFromDictionary(cachedRates, fromCurrency, toCurrency, out decimal rate))
                {
                    return rate;
                }
            }

            if (this.TryGetRateFromDictionary(this.exchangeRates, fromCurrency, toCurrency, out decimal defaultRate))
            {
                return defaultRate;
            }

            if (fromCurrency != "USD" && toCurrency != "USD")
            {
                if (this.TryGetRateFromDictionary(this.exchangeRates, fromCurrency, "USD", out decimal fromToUsd) &&
                    this.TryGetRateFromDictionary(this.exchangeRates, "USD", toCurrency, out decimal usdToTarget))
                {
                    return fromToUsd * usdToTarget;
                }
            }

            return 1.0m;
        }

        private bool TryGetRateFromDictionary(
            Dictionary<string, Dictionary<string, decimal>> rates,
            string fromCurrency,
            string toCurrency,
            out decimal rate)
        {
            rate = 1.0m;

            if (rates.TryGetValue(fromCurrency, out var fromRates) &&
                fromRates.TryGetValue(toCurrency, out var directRate))
            {
                rate = directRate;
                return true;
            }

            if (rates.TryGetValue(toCurrency, out var toRates) &&
                toRates.TryGetValue(fromCurrency, out var reverseRate) &&
                reverseRate != 0)
            {
                rate = 1 / reverseRate;
                return true;
            }

            return false;
        }

        private Dictionary<string, Dictionary<string, decimal>> InitializeExchangeRates()
        {
            var configRates = this.LoadRatesFromConfiguration();
            if (configRates != null && configRates.Count > 0)
            {
                return configRates;
            }

            var rates = new Dictionary<string, Dictionary<string, decimal>>(StringComparer.OrdinalIgnoreCase)
            {
                ["USD"] = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                {
                    ["EUR"] = 0.9173m,
                    ["GBP"] = 0.7700m,
                    ["JPY"] = 147.3374m,
                    ["CAD"] = 1.4375m,
                    ["AUD"] = 1.5858m,
                    ["BGN"] = 1.7926m,
                },
                ["EUR"] = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                {
                    ["USD"] = 1.0857m,
                    ["GBP"] = 0.8381m,
                    ["JPY"] = 160.3991m,
                    ["CAD"] = 1.5638m,
                    ["AUD"] = 1.7249m,
                    ["BGN"] = 1.9513m,
                },
                ["BGN"] = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                {
                    ["USD"] = 0.5539m,
                    ["EUR"] = 0.5094m,
                    ["GBP"] = 0.4276m,
                    ["JPY"] = 81.7555m,
                    ["CAD"] = 0.7970m,
                    ["AUD"] = 0.8798m,
                },
            };

            this.cache.Set(CacheKey, rates, this.cacheExpirationTime);

            return rates;
        }
    }
}