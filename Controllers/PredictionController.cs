using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Services;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly ExpensePredictionService _predictionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PredictionController(ExpensePredictionService predictionService, UserManager<ApplicationUser> userManager)
        {
            _predictionService = predictionService;
            _userManager = userManager;
        }

        [HttpGet("predict-expenses")]
        public async Task<IActionResult> PredictExpenses([FromQuery] int months = 3)
        {
            var userId = _userManager.GetUserId(User)!;
            try
            {
                var forecast    = await _predictionService.GetForecastAsync(userId, months);
                var predictions = forecast.Where(p => p.IsPredicted).Select(p => new { month = p.Month, amount = p.Amount });
                return Ok(new { predictions });
            }
            catch (InvalidOperationException ex) { return Ok(new { error = ex.Message }); }
        }

        [HttpGet("forecast-full")]
        public async Task<IActionResult> ForecastFull([FromQuery] int months = 3)
        {
            var userId = _userManager.GetUserId(User)!;
            try
            {
                var forecast = await _predictionService.GetForecastAsync(userId, months);
                return Ok(new { forecast });
            }
            catch (InvalidOperationException ex) { return Ok(new { error = ex.Message }); }
        }
    }
}
