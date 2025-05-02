namespace ExpenseDb.API.Models.Dtos
{
    public class DateRangeReportDto
    {
        public string Period { get; set; } = string.Empty; // Örn: "2025-04-22 - 2025-04-28"
        public decimal TotalAmount { get; set; }
    }
}
