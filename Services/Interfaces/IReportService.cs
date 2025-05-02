using ExpenseDb.API.Models.Dtos.Reports;

namespace ExpenseDb.API.Services
{
    public interface IReportService
    {
        Task<List<CategoryExpenseReportDto>> GetCategoryTotalsAsync(DateTime startDate, DateTime endDate);
    }
}
