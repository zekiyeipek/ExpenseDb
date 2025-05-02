namespace ExpenseDb.API.Models.Dtos
{
    public class ExpenseTrendDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
