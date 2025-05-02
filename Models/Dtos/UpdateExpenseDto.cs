using System.ComponentModel.DataAnnotations;

public class UpdateExpenseDto
{
    [Required]
    public string Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public int ExpenseCategoryId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}
