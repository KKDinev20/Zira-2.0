using System.Collections.Generic;

namespace Zira.Services.Currency.Internals
{
    public static class ExchangeRates
    {
        private static readonly Dictionary<(string From, string To), decimal> Rates =
            new Dictionary<(string From, string To), decimal>
            {
                { ("BGN", "USD"), 0.55m },
                { ("BGN", "EUR"), 0.51m },
                { ("BGN", "GBP"), 0.43m },
                { ("USD", "BGN"), 1.82m },
                { ("USD", "EUR"), 0.93m },
                { ("USD", "GBP"), 0.78m },
                { ("EUR", "BGN"), 1.95m },
                { ("EUR", "USD"), 1.08m },
                { ("EUR", "GBP"), 0.84m },
                { ("GBP", "BGN"), 2.33m },
                { ("GBP", "USD"), 1.28m },
                { ("GBP", "EUR"), 1.19m },
            };

        public static decimal GetRate(string fromCurrency, string toCurrency)
        {
            if (Rates.TryGetValue((fromCurrency.ToUpper(), toCurrency.ToUpper()), out decimal rate))
            {
                return rate;
            }

            return 0;
        }
    }
}