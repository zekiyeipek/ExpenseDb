public class ExpenseResponseDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CategoryName { get; set; } = string.Empty; 
    public DateTime CreatedAt { get; set; }
    public ExpenseStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
}
