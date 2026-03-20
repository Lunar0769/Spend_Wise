using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly ExpenseDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BudgetController(ExpenseDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId) ?? new Budget();
            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Budget model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var userId   = _userManager.GetUserId(User)!;
            var existing = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (existing == null)
            {
                model.UserId    = userId;
                model.UpdatedAt = DateTime.UtcNow;
                _context.Budgets.Add(model);
            }
            else
            {
                existing.MonthlyIncome   = model.MonthlyIncome;
                existing.ThresholdPercent = model.ThresholdPercent;
                existing.UpdatedAt       = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Budget settings saved successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
