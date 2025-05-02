using System.ComponentModel.DataAnnotations;

namespace ExpenseDb.API.Models.Dtos
{
    public class CreateExpenseDto
    {
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public required string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur.")]
        public int ExpenseCategoryId { get; set; }
    }
}
