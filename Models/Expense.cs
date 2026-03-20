using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")] 
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }

    public static class ExpenseCategories
    {
        public static readonly List<string> All = new()
        {
            "Food & Dining",
            "Transportation",
            "Shopping",
            "Entertainment",
            "Health & Fitness",
            "Bills & Utilities",
            "Travel",
            "Education",
            "Personal Care",
            "Other"
        };

        public static string GetIcon(string category) => category switch
        {
            "Food & Dining" => "🍽️",
            "Transportation" => "🚗",
            "Shopping" => "🛍️",
            "Entertainment" => "🎮",
            "Health & Fitness" => "💪",
            "Bills & Utilities" => "💡",
            "Travel" => "✈️",
            "Education" => "📚",
            "Personal Care" => "💄",
            _ => "📦"
        };

        public static string GetColor(string category) => category switch
        {
            "Food & Dining" => "#FF6B6B",
            "Transportation" => "#4ECDC4",
            "Shopping" => "#FFE66D",
            "Entertainment" => "#A8E6CF",
            "Health & Fitness" => "#FF8B94",
            "Bills & Utilities" => "#6C5CE7",
            "Travel" => "#74B9FF",
            "Education" => "#FDCB6E",
            "Personal Care" => "#FD79A8",
            _ => "#B2BEC3"
        };
    }
}
