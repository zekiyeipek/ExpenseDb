using System.ComponentModel.DataAnnotations;
using ExpenseDb.API.Models;

public enum ExpenseStatus
{
    Pending,
    Approved,
    Rejected
}

public class Expense
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

    public string? RejectionReason { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ExpenseCategoryId { get; set; }
    public ExpenseCategory ExpenseCategory { get; set; } = null!;

    public string? DocumentFileName { get; set; } // belge dosya adı


}
