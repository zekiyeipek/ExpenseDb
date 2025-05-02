namespace ExpenseDb.API.Models
{
    public class PaymentSimulation
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string Iban { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    }
}
