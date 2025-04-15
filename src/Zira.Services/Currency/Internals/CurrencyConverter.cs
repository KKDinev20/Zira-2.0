using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Zira.Data;
using Zira.Services.Currency.Contracts;

namespace Zira.Services.Currency
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private const string CacheKey = "ExchangeRates";
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;
        private readonly EntityContext context;
        private readonly TimeSpan cacheExpirationTime = TimeSpan.FromHours(24);

        public CurrencyConverter(
            IMemoryCache cache,
            IConfiguration configuration,
            EntityContext context)
        {
            this.cache = cache;
            this.configuration = configuration;
            this.context = context;
        }

        // Get the exchange rate for from and to currencies and calculate - Math.Round amount * rate, 2
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

            var rate = await this.GetExchangeRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }

        // Load rates from the database and if they exist return the rate
        // If neither are BGN return fromToBgn * bgnToTo
        //
        private async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
        {
            fromCurrency = fromCurrency.ToUpperInvariant();
            toCurrency = toCurrency.ToUpperInvariant();

            var cachedRates = await this.cache.GetOrCreateAsync(
                CacheKey,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = this.cacheExpirationTime;
                    return await this.LoadRatesFromDatabaseAsync();
                });

            if (this.TryGetRateFromDictionary(cachedRates, fromCurrency, toCurrency, out decimal rate))
            {
                return rate;
            }

            if (fromCurrency != "BGN" && toCurrency != "BGN")
            {
                if (this.TryGetRateFromDictionary(cachedRates, fromCurrency, "BGN", out decimal fromToBgn) &&
                    this.TryGetRateFromDictionary(cachedRates, "BGN", toCurrency, out decimal bgnToTo))
                {
                    return fromToBgn * bgnToTo;
                }
            }

            var exchangeRate = await this.context.ExchangeRates
                .Where(r => r.FromCurrencyCode == fromCurrency && r.ToCurrencyCode == toCurrency)
                .FirstOrDefaultAsync();

            if (exchangeRate != null)
            {
                return exchangeRate.Rate;
            }

            if (fromCurrency != "BGN" && toCurrency != "BGN")
            {
                var fromToBgn = await this.context.ExchangeRates
                    .Where(r => r.FromCurrencyCode == fromCurrency && r.ToCurrencyCode == "BGN")
                    .Select(r => r.Rate)
                    .FirstOrDefaultAsync();

                var bgnToTo = await this.context.ExchangeRates
                    .Where(r => r.FromCurrencyCode == "BGN" && r.ToCurrencyCode == toCurrency)
                    .Select(r => r.Rate)
                    .FirstOrDefaultAsync();

                if (fromToBgn != 0 && bgnToTo != 0)
                {
                    return fromToBgn * bgnToTo;
                }
            }

            return 1.0m;
        }

        // Get the rate from a dictionary
        // if TryGetValue for rates and from rates are equal map them
        // if its reverseRate rate = 1 / reverseRate
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

        // Get the rates from the dictionary and strign compare to ignore ordinal case
        // Get the exchange rate and for each rate check if it is contained, if not, add them in new dictionary
        // Rates[From][To] equals the rate of each exchange rate
        private async Task<Dictionary<string, Dictionary<string, decimal>>> LoadRatesFromDatabaseAsync()
        {
            var rates = new Dictionary<string, Dictionary<string, decimal>>(StringComparer.OrdinalIgnoreCase);

            var exchangeRates = await this.context.ExchangeRates.ToListAsync();

            foreach (var exchangeRate in exchangeRates)
            {
                if (!rates.ContainsKey(exchangeRate.FromCurrencyCode))
                {
                    rates[exchangeRate.FromCurrencyCode] =
                        new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                }

                rates[exchangeRate.FromCurrencyCode][exchangeRate.ToCurrencyCode] = exchangeRate.Rate;
            }

            return rates;
        }
    }
}