using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

var builder = WebApplication.CreateBuilder(args);

// Bind to Render's PORT env variable (falls back to 8080)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();

// PostgreSQL — must be registered BEFORE DataProtection so the DbContext is available
var rawConnString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No database connection string found.");

// Convert postgresql:// URI format to Npgsql key=value format
var connectionString = rawConnString;
if (rawConnString.StartsWith("postgresql://") || rawConnString.StartsWith("postgres://"))
{
    var uri = new Uri(rawConnString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;Pooling=false";
}

builder.Services.AddDbContext<ExpenseDbContext>(options =>
    options.UseNpgsql(connectionString));

// DataProtection keys stored in Postgres — AFTER DbContext registration
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ExpenseDbContext>()
    .SetApplicationName("SpendWise");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ExpenseDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/";
});

builder.Services.AddScoped<ExpenseTracker.Services.ExpensePredictionService>();

var app = builder.Build();

// Run migrations on startup — creates DataProtectionKeys table if not present
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
