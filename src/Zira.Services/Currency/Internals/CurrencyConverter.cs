using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zira.Data;
using Zira.Services.Currency.Contracts;
using Zira.Services.Currency.Exceptions;

namespace Zira.Services.Currency
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private const string CacheKey = "ExchangeRates";
        private const string PivotCurrency = "BGN";

        private readonly IMemoryCache cache;
        private readonly EntityContext context;
        private readonly ILogger<CurrencyConverter> logger;
        private readonly TimeSpan cacheExpirationTime = TimeSpan.FromHours(24);

        public CurrencyConverter(IMemoryCache cache, EntityContext context, ILogger<CurrencyConverter> logger)
        {
            this.cache = cache;
            this.context = context;
            this.logger = logger;
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

            var rates = await this.cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = this.cacheExpirationTime;
                return await this.LoadRatesFromDatabaseAsync();
            }) ?? new Dictionary<string, Dictionary<string, decimal>>();

            // Direct rate, or its inverse (handled inside TryGetRateFromDictionary)
            if (this.TryGetRateFromDictionary(rates, fromCurrency, toCurrency, out var rate))
            {
                return rate;
            }

            // Cross rate through the pivot currency
            if (fromCurrency != PivotCurrency && toCurrency != PivotCurrency
                                              && this.TryGetRateFromDictionary(rates, fromCurrency, PivotCurrency, out var fromToPivot)
                                              && this.TryGetRateFromDictionary(rates, PivotCurrency, toCurrency, out var pivotToTo))
            {
                return fromToPivot * pivotToTo;
            }

            this.logger.LogWarning("Missing exchange rate {From} -> {To}", fromCurrency, toCurrency);
            throw new ExchangeRateNotFoundException(fromCurrency, toCurrency);
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