namespace ExpenseDb.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string Iban { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Personel;

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }

    public enum Role
    {
        Admin = 1,
        Personel = 2
    }
}
