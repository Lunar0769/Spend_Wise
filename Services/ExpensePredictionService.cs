using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class PredictionResult
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPredicted { get; set; }
    }

    public class ExpensePredictionService
    {
        private readonly ExpenseDbContext _context;

        // Per-user in-memory cache
        private static readonly Dictionary<string, (List<PredictionResult> data, DateTime expiry)> _cache = new();
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public ExpensePredictionService(ExpenseDbContext context)
        {
            _context = context;
        }

        public async Task<List<PredictionResult>> GetForecastAsync(string userId, int futureMonths = 3)
        {
            if (_cache.TryGetValue(userId, out var cached) && DateTime.UtcNow < cached.expiry)
                return cached.data;

            var monthly = await GetMonthlyTotalsAsync(userId);

            if (monthly.Count < 3)
                throw new InvalidOperationException("Not enough data to generate predictions");

            var predictions = PredictFutureExpenses(monthly, futureMonths);

            var result = monthly
                .Select(m => new PredictionResult { Month = m.Label, Amount = Math.Round(m.Total, 2), IsPredicted = false })
                .Concat(predictions)
                .ToList();

            _cache[userId] = (result, DateTime.UtcNow.Add(CacheDuration));
            return result;
        }

        private async Task<List<MonthlyData>> GetMonthlyTotalsAsync(string userId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .Select(e => new { e.Date, e.Amount })
                .ToListAsync();

            return expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select((g, idx) => new MonthlyData
                {
                    Year  = g.Key.Year,
                    Month = g.Key.Month,
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Total = g.Sum(e => e.Amount),
                    Index = idx + 1
                })
                .ToList();
        }

        private static List<PredictionResult> PredictFutureExpenses(List<MonthlyData> history, int futureMonths)
        {
            var (slope, intercept) = LinearRegression(history);
            bool useMovingAverage  = history.Count < 6;
            var lastDate = new DateTime(history.Last().Year, history.Last().Month, 1);
            var results  = new List<PredictionResult>();

            for (int i = 1; i <= futureMonths; i++)
            {
                decimal predicted = useMovingAverage
                    ? history.TakeLast(Math.Min(3, history.Count)).Average(m => m.Total)
                    : (decimal)(slope * (history.Last().Index + i) + intercept);

                predicted = Math.Max(0, Math.Round(predicted, 2));
                results.Add(new PredictionResult
                {
                    Month       = lastDate.AddMonths(i).ToString("MMM yyyy"),
                    Amount      = predicted,
                    IsPredicted = true
                });
            }
            return results;
        }

        private static (double slope, double intercept) LinearRegression(List<MonthlyData> data)
        {
            int n = data.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            foreach (var d in data)
            {
                sumX  += d.Index;
                sumY  += (double)d.Total;
                sumXY += d.Index * (double)d.Total;
                sumX2 += d.Index * d.Index;
            }
            double denom = n * sumX2 - sumX * sumX;
            if (Math.Abs(denom) < 1e-10) return (0, sumY / n);
            double slope     = (n * sumXY - sumX * sumY) / denom;
            double intercept = (sumY - slope * sumX) / n;
            return (slope, intercept);
        }

        public static void InvalidateCache(string? userId = null)
        {
            if (userId != null) _cache.Remove(userId);
            else _cache.Clear();
        }

        private class MonthlyData
        {
            public int Year  { get; set; }
            public int Month { get; set; }
            public string Label { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public int Index { get; set; }
        }
    }
}
