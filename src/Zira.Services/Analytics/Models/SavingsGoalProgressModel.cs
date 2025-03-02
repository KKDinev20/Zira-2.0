namespace Zira.Services.Analytics.Models;

public class SavingsGoalProgressModel
{
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal Progress { get; set; } 
}