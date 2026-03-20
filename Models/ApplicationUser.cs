using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
