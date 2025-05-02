namespace ExpenseDb.API.Models.Dtos
{
    public class CategoryReportDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
