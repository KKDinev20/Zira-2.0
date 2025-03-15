using System;

namespace Zira.Data.Models
{
    public class ReminderSettings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool EnableBillReminders { get; set; } = true;
        public bool EnableBudgetAlerts { get; set; } = true;

        public ApplicationUser? User { get; set; }
    }
}