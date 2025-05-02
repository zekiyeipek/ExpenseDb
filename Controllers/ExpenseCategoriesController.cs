using Microsoft.AspNetCore.Mvc;
using ExpenseDb.API.Data;
using ExpenseDb.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseDb.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _context.ExpenseCategories.ToList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.ExpenseCategories.Find(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ExpenseCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Kategori adı boş olamaz.");

            _context.ExpenseCategories.Add(category);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ExpenseCategory updated)
        {
            var category = _context.ExpenseCategories.Find(id);
            if (category == null) return NotFound();

            category.Name = updated.Name;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _context.ExpenseCategories.Find(id);
            if (category == null) return NotFound("Kategori bulunamadı.");

            var hasExpenses = _context.Expenses.Any(e => e.ExpenseCategoryId == id);
            if (hasExpenses)
                return BadRequest("Bu kategoriye ait masraflar bulunduğu için silinemez.");

            _context.ExpenseCategories.Remove(category);
            _context.SaveChanges();

            return Ok("Kategori başarıyla silindi.");
        }


        [HttpDelete("{id}/remove")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.ExpenseCategories.Find(id);
            if (category == null) return NotFound("Kategori bulunamadı.");

            var hasExpenses = _context.Expenses.Any(e => e.ExpenseCategoryId == id);
            if (hasExpenses)
                return BadRequest("Bu kategoriye ait masraflar bulunduğu için silinemez.");

            _context.ExpenseCategories.Remove(category);
            _context.SaveChanges();

            return Ok("Kategori silindi.");
        }

    }
}
