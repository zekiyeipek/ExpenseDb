namespace ExpenseDb.API.Models.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Iban { get; set; } = string.Empty;
        public string Role { get; set; } = "Personel"; // varsayılan
    }
}
