using System.ComponentModel.DataAnnotations;
using Zira.Data.Enums;

namespace Zira.Presentation.Models
{
    public class NotificationSettingsViewModel
    {
        [Display(Name = "Enable Bill Reminders")]
        public bool EnableBillReminders { get; set; }

        [Display(Name = "Enable Budget Alerts")]
        public bool EnableBudgetAlerts { get; set; }

        [Display(Name = "Preferred Notification Method")]
        public NotificationType PreferredNotification { get; set; }
    }
}