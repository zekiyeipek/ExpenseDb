namespace ExpenseDb.API.Models
{
    public class ExpenseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
