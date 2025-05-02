using Microsoft.AspNetCore.Mvc;
using ExpenseDb.API.Data;
using ExpenseDb.API.Models;
using Microsoft.AspNetCore.Authorization;
using ExpenseDb.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ExpenseDb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var expenses = _context.Expenses.ToList();
            return Ok(expenses);
        }

        /// <summary>
        /// Yeni bir masraf talebi oluşturur.
        /// </summary>
        /// <remarks>
        /// Bu endpoint sadece giriş yapmış personel kullanıcılar tarafından erişilebilir.
        /// Zorunlu alanlar: Açıklama, Tutar, Geçerli bir kategori ID'si.
        /// </remarks>
        /// <param name="dto">Oluşturulacak masraf verisi</param>
        /// <response code="201">Masraf başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz veri gönderildi.</response>
        /// <response code="401">Yetkilendirme hatası.</response>
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            var category = _context.ExpenseCategories.FirstOrDefault(c => c.Id == dto.ExpenseCategoryId);
            if (category == null) return BadRequest("Geçerli bir kategori bulunamadı.");

            var expense = new Expense
            {
                Description = dto.Description,
                Amount = dto.Amount,
                ExpenseCategoryId = category.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ExpenseStatus.Pending,
                UserId = user.Id
            };

            _context.Expenses.Add(expense);
            _context.SaveChanges();

            var response = new ExpenseResponseDto
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                CategoryName = category.Name,
                CreatedAt = expense.CreatedAt,
                Status = expense.Status,
                UserId = user.Id,
                UserFullName = user.FullName
            };

            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, response);
        }


        /// <summary>
        /// Belirli bir masraf kaydının detayını getirir.
        /// </summary>
        /// <param name="id">Masraf ID</param>
        /// <response code="200">Masraf detayı</response>
        /// <response code="404">Masraf bulunamadıysa</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var expense = _context.Expenses
                .Where(e => e.Id == id)
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    CategoryName = e.ExpenseCategory.Name,
                    CreatedAt = e.CreatedAt,
                    Status = e.Status,
                    RejectionReason = e.RejectionReason,
                    UserId = e.UserId,
                    UserFullName = e.User.FullName
                })
                .FirstOrDefault();

            if (expense == null)
                return NotFound();

            return Ok(expense);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var expense = _context.Expenses.Find(id);
            if (expense == null)
                return NotFound();

            expense.Description = dto.Description;
            expense.Amount = dto.Amount;
            expense.ExpenseCategoryId = dto.ExpenseCategoryId;
            expense.CreatedAt = dto.CreatedAt;

            _context.SaveChanges();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost("my")]
        [Authorize(Roles = "Personel")]
        public IActionResult GetMyExpenses([FromBody] ExpenseFilterDto filter)
        {
            var email = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            var query = _context.Expenses
                .Where(e => e.UserId == user.Id);

            if (filter.StartDate.HasValue)
                query = query.Where(e => e.CreatedAt >= DateTime.SpecifyKind(filter.StartDate.Value, DateTimeKind.Utc));

            if (filter.EndDate.HasValue)
                query = query.Where(e => e.CreatedAt <= DateTime.SpecifyKind(filter.EndDate.Value, DateTimeKind.Utc));

            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(e => e.ExpenseCategory.Name == filter.Category);

            if (filter.Status.HasValue)
                query = query.Where(e => e.Status == filter.Status.Value);

            var expenses = query.Select(e => new ExpenseResponseDto
            {
                Id = e.Id,
                Description = e.Description,
                Amount = e.Amount,
                CategoryName = e.ExpenseCategory.Name,
                CreatedAt = e.CreatedAt,
                Status = e.Status,
                RejectionReason = e.RejectionReason,
                UserId = e.UserId,
                UserFullName = e.User.FullName
            }).ToList();

            return Ok(expenses);
        }


        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllExpenses()
        {
            var expenses = _context.Expenses
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    CategoryName = e.ExpenseCategory.Name,
                    CreatedAt = e.CreatedAt,
                    Status = e.Status,
                    RejectionReason = e.RejectionReason,
                    UserId = e.UserId,
                    UserFullName = e.User.FullName
                })
                .ToList();

            return Ok(expenses);
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public IActionResult RejectExpense(int id, [FromBody] string reason)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null) return NotFound();

            if (string.IsNullOrWhiteSpace(reason))
                return BadRequest("Red gerekçesi girilmelidir.");

            expense.Status = ExpenseStatus.Rejected;
            expense.RejectionReason = reason;

            _context.SaveChanges();
            return Ok("Masraf reddedildi.");
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveExpense(int id)
        {
            var expense = _context.Expenses.Include(e => e.User).FirstOrDefault(e => e.Id == id);
            if (expense == null) return NotFound();

            expense.Status = ExpenseStatus.Approved;
            expense.RejectionReason = null;

            // Simülasyon: Ödeme oluştur
            var payment = new PaymentSimulation
            {
                ExpenseId = expense.Id,
                Amount = expense.Amount,
                PaidAt = DateTime.UtcNow,
                Iban = expense.User.Iban
            };

            _context.PaymentSimulations.Add(payment);
            _context.SaveChanges();

            return Ok("Masraf onaylandı ve ödeme simülasyonu oluşturuldu.");
        }


        [HttpPost("{id}/simulate-payment")]
        [Authorize(Roles = "Admin")]
        public IActionResult SimulatePayment(int id)
        {
            var expense = _context.Expenses
                .Include(e => e.User)
                .FirstOrDefault(e => e.Id == id && e.Status == ExpenseStatus.Approved);

            if (expense == null)
                return NotFound("Onaylanmış masraf bulunamadı.");

            var existingPayment = _context.PaymentSimulations.FirstOrDefault(p => p.ExpenseId == id);
            if (existingPayment != null)
                return BadRequest("Bu masraf için ödeme zaten simüle edilmiş.");

            var payment = new PaymentSimulation
            {
                ExpenseId = expense.Id,
                Iban = expense.User.Iban,
                Amount = expense.Amount,
                PaidAt = DateTime.UtcNow
            };

            _context.PaymentSimulations.Add(payment);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Ödeme başarıyla simüle edildi.",
                Payment = payment
            });
        }
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> CreateWithDocument([FromForm] CreateExpenseDto dto, IFormFile? document)
        {
            var email = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            string? fileName = null;
            if (document != null && document.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                fileName = $"{Guid.NewGuid()}_{document.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await document.CopyToAsync(stream);
            }

            var expense = new Expense
            {
                Description = dto.Description,
                Amount = dto.Amount,
                ExpenseCategoryId = dto.ExpenseCategoryId,
                CreatedAt = DateTime.UtcNow,
                Status = ExpenseStatus.Pending,
                UserId = user.Id,
                DocumentFileName = fileName
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Masraf kaydı başarıyla oluşturuldu.", FileName = fileName });
        }


    }
}
