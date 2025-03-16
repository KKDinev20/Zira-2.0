using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zira.Services.Currency.Contracts
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertCurrencyAsync(Guid userId, decimal amount, string fromCurrency, string toCurrency);
        
    }
}