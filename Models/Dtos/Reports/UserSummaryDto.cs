namespace ExpenseDb.API.Models.Dtos
{
    public class UserSummaryDto
    {
        public string FullName { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
