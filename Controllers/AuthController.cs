using Microsoft.AspNetCore.Mvc;
using ExpenseDb.API.Models;
using ExpenseDb.API.Models.Auth;
using ExpenseDb.API.Data;
using ExpenseDb.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseDb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _auth;

        public AuthController(AppDbContext context, AuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta ile kayıtlı kullanıcı zaten var.");

            _auth.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Iban = dto.Iban,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = Enum.Parse<Role>(dto.Role, true)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (user.PasswordHash == null || user.PasswordSalt == null)
                return Unauthorized("Şifre verisi eksik.");
            if (!_auth.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Şifre hatalı.");

            var token = _auth.CreateToken(user);
            return Ok(new { token });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new 
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Iban
                })
                .ToList();

            return Ok(users);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] CreateUserDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta zaten kayıtlı.");

            _auth.CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Role = (Role)dto.Role,
                Iban = dto.Iban,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

    }
}
