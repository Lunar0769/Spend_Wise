using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Income must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }

        /// <summary>Percentage of income to treat as the spending threshold (e.g. 80 = 80%)</summary>
        [Required]
        [Range(1, 100, ErrorMessage = "Threshold must be between 1 and 100")]
        public int ThresholdPercent { get; set; } = 80;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // Computed helpers (not stored)
        [NotMapped]
        public decimal ThresholdAmount => Math.Round(MonthlyIncome * ThresholdPercent / 100m, 2);
    }
}
