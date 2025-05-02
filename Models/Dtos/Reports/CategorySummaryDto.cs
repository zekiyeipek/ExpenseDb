namespace ExpenseDb.API.Models.Dtos
{
    public class CategorySummaryDto
    {
        public string Category { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
