using System;
using System.Collections.Generic;

namespace Zira.Data.Models
{
    public class SavingsGoal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? TargetDate { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Transaction> RelatedTransactions { get; set; }
    }
}