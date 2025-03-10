using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Zira.Data.Models;
using Zira.Services.Currency.Contracts;

namespace Zira.Services.Currency.Internals;

public class CurrencyConverter : ICurrencyConverter
{
    private readonly UserManager<ApplicationUser> userManager;

    public CurrencyConverter(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<decimal> ConvertCurrencyAsync(Guid userId, decimal amount, string fromCurrency)
    {
        var user = await this.userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        if (string.IsNullOrEmpty(user.PreferredCurrency))
        {
            throw new ArgumentException("User preferred currency is not set.");
        }

        if (string.IsNullOrEmpty(fromCurrency))
        {
            throw new ArgumentException("From currency is not set.");
        }

        if (user.PreferredCurrency.Equals(fromCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return amount;
        }

        return this.Convert(fromCurrency, user.PreferredCurrency, amount);
    }

    private decimal Convert(string fromCurrency, string toCurrency, decimal amount)
    {
        decimal conversionRate = ExchangeRates.GetRate(fromCurrency, toCurrency);

        if (conversionRate == 0)
        {
            throw new ArgumentException($"Conversion rate not found for {fromCurrency} to {toCurrency}.");
        }

        return amount * conversionRate;
    }
}