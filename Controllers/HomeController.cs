using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExpenseDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ExpenseDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var now = DateTime.Today;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfYear  = new DateTime(now.Year, 1, 1);

            var allExpenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var thisMonthExpenses = allExpenses.Where(e => e.Date >= startOfMonth).ToList();
            var thisYearExpenses  = allExpenses.Where(e => e.Date >= startOfYear).ToList();
            var daysInMonth = (now - startOfMonth).Days + 1;

            var monthlyTotals = new List<MonthlyTotal>();
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var start = new DateTime(month.Year, month.Month, 1);
                var end   = start.AddMonths(1);
                var spent = allExpenses.Where(e => e.Date >= start && e.Date < end).Sum(e => e.Amount);
                var saved = 0m;
                if (budget != null && budget.MonthlyIncome > 0)
                {
                    var spendable = budget.ThresholdAmount;
                    var savings   = budget.MonthlyIncome - spendable;
                    saved = spent <= spendable
                        ? savings                          // full savings intact
                        : Math.Max(0, savings - (spent - spendable)); // savings dipped into
                }
                monthlyTotals.Add(new MonthlyTotal
                {
                    Month       = month.ToString("MMM"),
                    Year        = month.Year,
                    MonthNumber = month.Month,
                    Amount      = spent,
                    Saved       = saved
                });
            }

            var categoryTotals = thisMonthExpenses
                .GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

            var topCategory = categoryTotals.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId);

            // Budget breakdown
            decimal spendableAmount = 0, spendableUsed = 0, savingsUsed = 0, remainingSpendable = 0, remainingSavings = 0;
            decimal totalSavedAllTime = 0;
            string budgetStatus = string.Empty;
            var totalExpenses = thisMonthExpenses.Sum(e => e.Amount);

            if (budget != null && budget.MonthlyIncome > 0)
            {
                var spendable = budget.ThresholdAmount; // threshold = spendable
                var savings   = budget.MonthlyIncome - spendable; // remainder = savings

                spendableAmount = spendable;

                if (totalExpenses <= spendable)
                {
                    spendableUsed      = totalExpenses;
                    savingsUsed        = 0;
                    remainingSpendable = spendable - totalExpenses;
                    remainingSavings   = savings;
                }
                else
                {
                    spendableUsed      = spendable;
                    savingsUsed        = totalExpenses - spendable;
                    remainingSpendable = 0;
                    remainingSavings   = Math.Max(0, savings - savingsUsed);
                }

                budgetStatus = totalExpenses <= spendable * 0.7m ? "On Track"
                    : totalExpenses <= spendable               ? "Approaching Limit"
                    : remainingSavings > 0                     ? "Using Savings"
                    : "Overspent";

                // Total saved across all months (every month where expenses < spendable, savings bucket was intact)
                var allMonths = allExpenses
                    .GroupBy(e => new { e.Date.Year, e.Date.Month })
                    .Select(g => new { Spent = g.Sum(e => e.Amount) });

                foreach (var m in allMonths)
                {
                    var monthlySaved = m.Spent <= spendable
                        ? (budget.MonthlyIncome - spendable)
                        : Math.Max(0, (budget.MonthlyIncome - spendable) - (m.Spent - spendable));
                    totalSavedAllTime += monthlySaved;
                }
            }

            var vm = new DashboardViewModel
            {
                RecentExpenses        = allExpenses.OrderByDescending(e => e.Date).Take(8).ToList(),
                TotalThisMonth        = totalExpenses,
                TotalThisYear         = thisYearExpenses.Sum(e => e.Amount),
                TotalAllTime          = allExpenses.Sum(e => e.Amount),
                AverageDailyThisMonth = daysInMonth > 0 ? totalExpenses / daysInMonth : 0,
                CategoryTotals        = categoryTotals,
                MonthlyTotals         = monthlyTotals,
                TopCategory           = topCategory,
                TotalTransactions     = allExpenses.Count,
                Budget                = budget,
                SpendableAmount       = spendableAmount,
                SpendableUsed         = spendableUsed,
                SavingsUsed           = savingsUsed,
                RemainingSpendable    = remainingSpendable,
                RemainingSavings      = remainingSavings,
                TotalSavedAllTime     = totalSavedAllTime,
                BudgetStatus          = budgetStatus
            };

            return View(vm);
        }
    }
}
