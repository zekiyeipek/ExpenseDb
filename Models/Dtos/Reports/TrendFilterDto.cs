namespace ExpenseDb.API.Models.Dtos
{
    public class TrendFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = "daily"; // daily | weekly | monthly
    }
}
