namespace ExpenseTracker.Models
{
    public class DashboardViewModel
    {
        public List<Expense> RecentExpenses { get; set; } = new();
        public decimal TotalThisMonth { get; set; }
        public decimal TotalThisYear { get; set; }
        public decimal TotalAllTime { get; set; }
        public decimal AverageDailyThisMonth { get; set; }
        public Dictionary<string, decimal> CategoryTotals { get; set; } = new();
        public List<MonthlyTotal> MonthlyTotals { get; set; } = new();
        public string? TopCategory { get; set; }
        public int TotalTransactions { get; set; }
        public Budget? Budget { get; set; }

        // Budget breakdown
        public decimal SpendableAmount { get; set; }
        public decimal SpendableUsed { get; set; }
        public decimal SavingsUsed { get; set; }
        public decimal RemainingSpendable { get; set; }
        public decimal RemainingSavings { get; set; }
        public string BudgetStatus { get; set; } = string.Empty;
    }

    public class MonthlyTotal
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Saved { get; set; }
        public int Year { get; set; }
        public int MonthNumber { get; set; }
    }

    public class ExpenseFilterViewModel
    {
        public List<Expense> Expenses { get; set; } = new();
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "Date";
        public string SortOrder { get; set; } = "desc";
        public decimal TotalAmount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class RegisterViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string DisplayName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 6)]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
