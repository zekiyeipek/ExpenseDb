using Dapper;
using ExpenseDb.API.Models.Dtos.Reports;
using Npgsql;
using System.Data;

namespace ExpenseDb.API.Services
{
    public class ReportService : IReportService
    {
        private readonly IConfiguration _configuration;
        

        public ReportService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<CategoryExpenseReportDto>> GetCategoryTotalsAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = @"
                SELECT ec.""Name"" AS ""CategoryName"", SUM(e.""Amount"") AS ""TotalAmount""
                FROM ""Expenses"" e
                JOIN ""ExpenseCategories"" ec ON ec.""Id"" = e.""ExpenseCategoryId""
                WHERE e.""CreatedAt"" BETWEEN @StartDate AND @EndDate
                GROUP BY ec.""Name""";

            var result = await connection.QueryAsync<CategoryExpenseReportDto>(sql, new { StartDate = startDate, EndDate = endDate });
            return result.ToList();
        }
    }
}
