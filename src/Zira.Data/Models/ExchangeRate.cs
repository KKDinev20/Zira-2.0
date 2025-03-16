using System;

namespace Zira.Data.Models;

public class ExchangeRate
{
    public int Id { get; set; }
    public string FromCurrencyCode { get; set; }
    public string ToCurrencyCode { get; set; }
    public decimal Rate { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public Currency FromCurrency { get; set; }
    public Currency ToCurrency { get; set; }
}
