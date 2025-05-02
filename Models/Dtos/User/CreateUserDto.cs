public class CreateUserDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public int Role { get; set; } // 1: Admin, 2: Personel
}
