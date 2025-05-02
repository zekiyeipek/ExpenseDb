public class ExpenseFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Category { get; set; }
    public ExpenseStatus? Status { get; set; }
}
