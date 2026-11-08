using System;

namespace Zira.Services.Currency.Exceptions;

public class ExchangeRateNotFoundException : Exception
{
    public ExchangeRateNotFoundException(string fromCurrency, string toCurrency)
        : base($"No exchange rate from {fromCurrency} to {toCurrency}.")
    {
        this.FromCurrency = fromCurrency;
        this.ToCurrency = toCurrency;
    }

    public string FromCurrency { get; }

    public string ToCurrency { get; }
}