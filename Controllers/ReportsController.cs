using System.Globalization;
using ExpenseDb.API.Data;
using ExpenseDb.API.Models.Dtos;
using ExpenseDb.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly IReportService _reportService;

    public ReportsController(AppDbContext context, IReportService reportService)
    {
        _context = context;
        _reportService = reportService;
    }


    [HttpPost("my")]
    [Authorize(Roles = "Personel")]
    public IActionResult GetMyReport([FromBody] ExpenseFilterDto filter)
    {
        var email = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return Unauthorized();

        var query = _context.Expenses
            .Where(e => e.UserId == user.Id);

        if (filter.StartDate.HasValue)
            query = query.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            query = query.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var report = query
            .GroupBy(e => e.ExpenseCategory.Name)
            .Select(g => new
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount),
                Count = g.Count()
            })
            .ToList();

        return Ok(report);
    }

    [HttpPost("summary-by-personnel")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetSummaryByPersonnel([FromBody] ReportFilterDto filter)
    {
        var start = DateTime.SpecifyKind(filter.StartDate ?? DateTime.MinValue, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(filter.EndDate ?? DateTime.MaxValue, DateTimeKind.Utc);

        var summary = _context.Expenses
            .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
            .GroupBy(e => new { e.UserId, e.User.FullName })
            .Select(g => new PersonnelExpenseSummaryDto
            {
                UserId = g.Key.UserId,
                FullName = g.Key.FullName,
                TotalAmount = g.Sum(x => x.Amount)
            })
            .ToList();

        return Ok(summary);
    }

    [HttpPost("trend")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetExpenseTrend([FromBody] TrendFilterDto filter)
    {
        if (string.IsNullOrWhiteSpace(filter.GroupBy))
            return BadRequest("GroupBy parametresi gereklidir: 'daily', 'weekly' veya 'monthly'.");

        var expenses = _context.Expenses
            .Where(e => e.CreatedAt >= filter.StartDate && e.CreatedAt <= filter.EndDate)
            .ToList();

        var grouped = filter.GroupBy.ToLower() switch
        {
            "daily" => expenses.GroupBy(e => e.CreatedAt.Date.ToString("yyyy-MM-dd")),
            "weekly" => expenses.GroupBy(e =>
                System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    e.CreatedAt, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString()),
            "monthly" => expenses.GroupBy(e => e.CreatedAt.ToString("yyyy-MM")),
            _ => throw new ArgumentException("Geçersiz gruplama türü.")
        };

        var result = grouped.Select(g => new ExpenseTrendDto
        {
            Period = g.Key,
            TotalAmount = g.Sum(e => e.Amount)
        }).OrderBy(x => x.Period).ToList();

        return Ok(result);
    }

    [HttpPost("my-by-category")]
    [Authorize(Roles = "Personel")]
    public IActionResult GetMyExpensesGroupedByCategory([FromBody] ReportFilterDto filter)
    {
        var email = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return Unauthorized();

        var expenses = _context.Expenses
            .Include(e => e.ExpenseCategory)
            .Where(e => e.UserId == user.Id);

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));
        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));
        if (!string.IsNullOrWhiteSpace(filter.Category))
            expenses = expenses.Where(e => e.ExpenseCategory.Name == filter.Category);
        if (filter.Status.HasValue)
            expenses = expenses.Where(e => e.Status == filter.Status.Value);

        var grouped = expenses
            .GroupBy(e => e.ExpenseCategory.Name)
            .Select(g => new CategoryReportDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToList();

        return Ok(grouped);
    }

    [HttpPost("weekly-summary")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetWeeklyExpenseSummary([FromBody] ReportFilterDto filter)
    {
        var expenses = _context.Expenses.AsQueryable();

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var weeklySummary = expenses
            .AsEnumerable() // EF Core PostgreSQL'de DateTime manipülasyonunu sınırlı desteklediği için
            .GroupBy(e => ISOWeek.ToDateTime(ISOWeek.GetYear(e.CreatedAt), ISOWeek.GetWeekOfYear(e.CreatedAt), DayOfWeek.Monday))
            .Select(g => new DateRangeReportDto
            {
                Period = $"{g.Key:yyyy-MM-dd} - {g.Key.AddDays(6):yyyy-MM-dd}",
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToList();

        return Ok(weeklySummary);
    }

    [HttpPost("monthly-summary")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetMonthlyExpenseSummary([FromBody] ReportFilterDto filter)
    {
        var expenses = _context.Expenses.AsQueryable();

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var monthlySummary = expenses
            .AsEnumerable()
            .GroupBy(e => new { e.CreatedAt.Year, e.CreatedAt.Month })
            .Select(g => new DateRangeReportDto
            {
                Period = $"{g.Key.Year}-{g.Key.Month:00}",
                TotalAmount = g.Sum(e => e.Amount)
            })
            .OrderBy(g => g.Period)
            .ToList();

        return Ok(monthlySummary);
    }

    [HttpPost("category-summary")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCategoryExpenseSummary([FromBody] ReportFilterDto filter)
    {
        var expenses = _context.Expenses.Include(e => e.ExpenseCategory).AsQueryable();

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var summary = expenses
            .GroupBy(e => e.ExpenseCategory.Name)
            .Select(g => new CategorySummaryDto
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToList();

        return Ok(summary);
    }

    [HttpPost("person-summary")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUserExpenseSummary([FromBody] ReportFilterDto filter)
    {
        var expenses = _context.Expenses
            .Include(e => e.User)
            .AsQueryable();

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var summary = expenses
            .GroupBy(e => e.User.FullName)
            .Select(g => new UserSummaryDto
            {
                FullName = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .ToList();

        return Ok(summary);
    }

    [HttpPost("status-summary")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetStatusSummary([FromBody] ReportFilterDto filter)
    {
        var expenses = _context.Expenses.AsQueryable();

        if (filter.StartDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

        if (filter.EndDate.HasValue)
            expenses = expenses.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        var approvedCount = expenses.Count(e => e.Status == ExpenseStatus.Approved);
        var rejectedCount = expenses.Count(e => e.Status == ExpenseStatus.Rejected);
        var approvedTotal = expenses.Where(e => e.Status == ExpenseStatus.Approved).Sum(e => e.Amount);
        var rejectedTotal = expenses.Where(e => e.Status == ExpenseStatus.Rejected).Sum(e => e.Amount);

        return Ok(new
        {
            ApprovedCount = approvedCount,
            ApprovedTotal = approvedTotal,
            RejectedCount = rejectedCount,
            RejectedTotal = rejectedTotal
        });
    }

    [HttpPost("category-totals")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCategoryTotals([FromBody] ReportFilterDto filter)
    {
        if (!filter.StartDate.HasValue || !filter.EndDate.HasValue)
            return BadRequest("Tarih aralığı belirtilmelidir.");

        var result = await _reportService.GetCategoryTotalsAsync(
            DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc),
            DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

        return Ok(result);
    }

    [HttpGet("my/export")]
    [Authorize(Roles = "Personel")]
    public IActionResult ExportMyExpensesAsCsv()
    {
        var email = User.Identity?.Name;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return Unauthorized();

        var expenses = _context.Expenses
            .Where(e => e.UserId == user.Id)
            .Include(e => e.ExpenseCategory)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();

        var csv = new StringWriter();
        csv.WriteLine("Tarih,Açıklama,Tutar,Kategori,Durum");

        foreach (var e in expenses)
        {
            string status = e.Status.ToString();
            csv.WriteLine($"{e.CreatedAt:yyyy-MM-dd},{e.Description},{e.Amount},{e.ExpenseCategory.Name},{status}");
        }

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        return File(fileBytes, "text/csv", "masraflar.csv");
    }

    [HttpGet("all/export")]
    [Authorize(Roles = "Admin")]
    public IActionResult ExportAllExpensesAsCsv()
    {
        var expenses = _context.Expenses
            .Include(e => e.User)
            .Include(e => e.ExpenseCategory)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();

        var csv = new StringWriter();
        csv.WriteLine("Kullanıcı,Tarih,Açıklama,Tutar,Kategori,Durum");

        foreach (var e in expenses)
        {
            string status = e.Status.ToString();
            csv.WriteLine($"{e.User.FullName},{e.CreatedAt:yyyy-MM-dd},{e.Description},{e.Amount},{e.ExpenseCategory.Name},{status}");
        }

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        return File(fileBytes, "text/csv", "tum-masraflar.csv");
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetDashboardSummary()
    {
        var totalExpense = _context.Expenses.Sum(e => e.Amount);
        var totalApproved = _context.Expenses.Where(e => e.Status == ExpenseStatus.Approved).Sum(e => e.Amount);
        var totalRejected = _context.Expenses.Where(e => e.Status == ExpenseStatus.Rejected).Sum(e => e.Amount);
        var pendingCount = _context.Expenses.Count(e => e.Status == ExpenseStatus.Pending);

        var userCount = _context.Users.Count();
        var categoryCount = _context.ExpenseCategories.Count();
        var today = DateTime.UtcNow.Date;
        var todayExpenses = _context.Expenses.Count(e => e.CreatedAt.Date == today);

        return Ok(new
        {
            ToplamMasraf = totalExpense,
            OnaylananMasraf = totalApproved,
            ReddedilenMasraf = totalRejected,
            BekleyenTalepSayısı = pendingCount,
            KullanıcıSayısı = userCount,
            KategoriSayısı = categoryCount,
            BugünküMasrafSayısı = todayExpenses
        });
    }





}
