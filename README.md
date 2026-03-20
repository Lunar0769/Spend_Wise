# 💸 SpendWise — ASP.NET MVC Expense Tracker

A multi-user expense tracking application built with ASP.NET Core 8 MVC, Entity Framework Core, ASP.NET Core Identity, and PostgreSQL. Features a clean modern UI with analytics charts, budget tracking, and AI-powered spending forecasts.

---

## Features

- Multi-user accounts — register, login, logout; all data is fully isolated per user
- Dashboard — monthly spend stats, bar chart (last 6 months), doughnut chart by category, recent activity
- Budget & Income — set monthly income and a spending threshold %; live progress bar with color-coded alerts
- Add / Edit / Delete expenses with full validation
- Filter & Search — by category, keyword, date range
- Sorting — by date, amount, title, category
- Pagination — 10 items per page
- AI Forecast — linear regression + moving average prediction for next 3 months
- Toast notifications on successful actions
- Responsive sidebar layout

---

## Project Structure

```
ExpenseTracker/
├── Controllers/
│   ├── AccountController.cs      # Register, Login, Logout
│   ├── HomeController.cs         # Dashboard (scoped to current user)
│   ├── ExpenseController.cs      # CRUD + filtering (scoped to current user)
│   ├── BudgetController.cs       # Income & threshold settings (per user)
│   └── PredictionController.cs   # Forecast API (per user)
├── Models/
│   ├── ApplicationUser.cs        # Identity user with DisplayName
│   ├── Expense.cs                # Expense entity + category helpers
│   ├── Budget.cs                 # Budget entity (income + threshold)
│   └── ViewModels.cs             # Dashboard, filter, register, login VMs
├── Data/
│   └── ExpenseDbContext.cs       # IdentityDbContext + Expense + Budget
├── Migrations/                   # EF Core migrations (auto-applied on startup)
├── Services/
│   └── ExpensePredictionService.cs
├── Views/
│   ├── Account/Login.cshtml
│   ├── Account/Register.cshtml
│   ├── Shared/_Layout.cshtml
│   ├── Home/Index.cshtml
│   ├── Budget/Index.cshtml
│   └── Expense/ (Index, Create, Edit)
├── Program.cs
├── appsettings.json
├── render.yaml                   # Render deployment config
└── ExpenseTracker.csproj
```

---

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL running locally (or use a free Supabase project)

### Setup

```bash
# 1. Set your local connection string in appsettings.json
#    "DefaultConnection": "Host=localhost;Port=5432;Database=spendwise;Username=postgres;Password=yourpassword"

# 2. Restore packages
dotnet restore

# 3. Apply migrations (creates tables)
dotnet ef database update

# 4. Run
dotnet run
# Open http://localhost:5000
```

---

## Deploying to Render + Supabase (Free Tier)

### Step 1 — Create a Supabase database

1. Go to [supabase.com](https://supabase.com) and create a free account
2. Create a new project (choose a region close to you)
3. Once created, go to: **Project Settings → Database**
4. Scroll to **Connection string → URI** tab
5. Copy the connection string — it looks like:
   ```
   postgresql://postgres:[YOUR-PASSWORD]@db.xxxxxxxxxxxx.supabase.co:5432/postgres
   ```
6. Replace `[YOUR-PASSWORD]` with your actual project password

> Important: Supabase requires SSL. The connection string above already includes it by default.

---

### Step 2 — Deploy to Render

1. Push your project to a GitHub repository
2. Go to [render.com](https://render.com) and create a free account
3. Click **New → Web Service**
4. Connect your GitHub repo
5. Render will auto-detect `render.yaml` — confirm these settings:
   - Build Command: `dotnet publish -c Release -o out`
   - Start Command: `dotnet out/ExpenseTracker.dll`
   - Runtime: `.NET`
6. Under **Environment Variables**, add:

   | Key | Value |
   |-----|-------|
   | `DATABASE_URL` | your Supabase connection string from Step 1 |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

7. Click **Deploy**

Render will build the app, run `db.Database.Migrate()` on startup to create all tables automatically, and serve it on a public URL.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8 MVC |
| Auth | ASP.NET Core Identity |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (Supabase) |
| Charts | Chart.js 4.4 (CDN) |
| Fonts | Google Fonts (Inter) |
| Hosting | Render (free tier) |

---

## Data Model

```
ApplicationUser  (extends IdentityUser)
  └── DisplayName

Expense
  ├── Id, Title, Amount, Category, Date, Notes, CreatedAt
  └── UserId → ApplicationUser

Budget
  ├── Id, MonthlyIncome, ThresholdPercent, UpdatedAt
  └── UserId → ApplicationUser
```

### Expense Categories
Food & Dining · Transportation · Shopping · Entertainment · Health & Fitness · Bills & Utilities · Travel · Education · Personal Care · Other

---

## Authentication Flow

1. Unauthenticated users are redirected to `/Account/Login`
2. New users register at `/Account/Register`
3. All controllers are decorated with `[Authorize]`
4. Every DB query is filtered by the current user's ID from Identity claims
5. Logout via the "Sign Out" button in the topbar

---

## Environment Variables Reference

| Variable | Description |
|----------|-------------|
| `DATABASE_URL` | Full PostgreSQL connection string (required in production) |
| `ASPNETCORE_ENVIRONMENT` | Set to `Production` on Render |
| `PORT` | Auto-set by Render; app binds to this automatically |

A multi-user expense tracking application built with ASP.NET Core 8 MVC, Entity Framework Core, ASP.NET Core Identity, and SQLite. Features a clean modern UI with analytics charts, budget tracking, and AI-powered spending forecasts.

---

## Features

- Multi-user accounts — register, login, logout; all data is fully isolated per user
- Dashboard — monthly spend stats, bar chart (last 6 months), doughnut chart by category, recent activity
- Budget & Income — set monthly income and a spending threshold %; live progress bar with color-coded alerts
- Add / Edit / Delete expenses with full validation
- Filter & Search — by category, keyword, date range
- Sorting — by date, amount, title, category
- Pagination — 10 items per page
- AI Forecast — linear regression + moving average prediction for next 3 months
- Toast notifications on successful actions
- Responsive sidebar layout

---

## Project Structure

```
ExpenseTracker/
├── Controllers/
│   ├── AccountController.cs      # Register, Login, Logout
│   ├── HomeController.cs         # Dashboard (scoped to current user)
│   ├── ExpenseController.cs      # CRUD + filtering (scoped to current user)
│   ├── BudgetController.cs       # Income & threshold settings (per user)
│   └── PredictionController.cs   # Forecast API (per user)
├── Models/
│   ├── ApplicationUser.cs        # Identity user with DisplayName
│   ├── Expense.cs                # Expense entity + category helpers
│   ├── Budget.cs                 # Budget entity (income + threshold)
│   └── ViewModels.cs             # Dashboard, filter, register, login VMs
├── Data/
│   └── ExpenseDbContext.cs       # IdentityDbContext + Expense + Budget
├── Services/
│   └── ExpensePredictionService.cs  # Per-user forecast logic
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Shared/_Layout.cshtml     # Sidebar layout, topbar with user/logout
│   ├── Home/Index.cshtml         # Dashboard
│   ├── Budget/Index.cshtml       # Budget settings
│   └── Expense/
│       ├── Index.cshtml
│       ├── Create.cshtml
│       └── Edit.cshtml
├── Program.cs                    # App entry + Identity + EF setup
├── appsettings.json
└── ExpenseTracker.csproj
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run Locally

```bash
# 1. Restore packages
dotnet restore

# 2. Run (DB auto-creates on first launch)
dotnet run

# 3. Open browser
# http://localhost:5000
```

On first run, go to `/Account/Register` to create your account. Each user's expenses, budget, and forecasts are completely separate.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8 MVC |
| Auth | ASP.NET Core Identity |
| ORM | Entity Framework Core 8 |
| Database | SQLite |
| Charts | Chart.js 4.4 (CDN) |
| Fonts | Google Fonts (Inter) |
| CSS | Custom CSS variables (no Bootstrap) |

---

## Data Model

```
ApplicationUser  (extends IdentityUser)
  └── DisplayName

Expense
  ├── Id, Title, Amount, Category, Date, Notes, CreatedAt
  └── UserId → ApplicationUser

Budget
  ├── Id, MonthlyIncome, ThresholdPercent, UpdatedAt
  └── UserId → ApplicationUser
```

### Expense Categories
Food & Dining · Transportation · Shopping · Entertainment · Health & Fitness · Bills & Utilities · Travel · Education · Personal Care · Other

---

## Authentication Flow

1. Unauthenticated users are redirected to `/Account/Login`
2. New users register at `/Account/Register`
3. All controllers are decorated with `[Authorize]`
4. Every DB query is filtered by the current user's ID from Identity claims
5. Logout via the "Sign Out" button in the topbar

---

## Customization

- Change database: Swap SQLite for SQL Server in `Program.cs` and install `Microsoft.EntityFrameworkCore.SqlServer`
- Add email confirmation: Set `options.SignIn.RequireConfirmedAccount = true` and configure an SMTP provider
- Add roles/admin: Use `RoleManager<IdentityRole>` to create roles and `[Authorize(Roles = "Admin")]` on controllers
- Export to CSV: Add an `ExportController` using the `CsvHelper` NuGet package
