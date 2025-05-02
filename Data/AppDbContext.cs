using Microsoft.EntityFrameworkCore;
using ExpenseDb.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseDb.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<User> Users => Set<User>();
        public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
        public DbSet<PaymentSimulation> PaymentSimulations => Set<PaymentSimulation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseCategory>().HasData(
                new ExpenseCategory { Id = 1, Name = "Ulaşım" },
                new ExpenseCategory { Id = 2, Name = "Yemek" },
                new ExpenseCategory { Id = 3, Name = "Konaklama" }
            );

            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Admin Kullanıcı",
                Email = "admin@example.com",
                Iban = "TR000000000000000000000000",
                PasswordHash = Convert.FromBase64String("nmSnf9k7evTG1hSjyRtM3S8ul/nXKsdqszEtQUOX5To="),
                PasswordSalt = Convert.FromBase64String("bGFh6oUi99/4PhZFEZ+gYXle9z2l8dEExAGyxm3fb4c3dKRFiEG9+64HRixow7ABlj5cybtz5goDYYj5rLzpRw=="),
                Role = Role.Admin
            },
            new User
            {
                Id = 2,
                FullName = "Personel Kullanıcı",
                Email = "personel@example.com",
                Iban = "TR111111111111111111111111",
                PasswordHash = Convert.FromBase64String("ZPuMc1lq6RekeLwXw3D0qKpAC6QX2Er4wFAFgDTMEHc="),
                PasswordSalt = Convert.FromBase64String("2x9kKn5Kr4mE1uWcKoZnJgFLULSeCjqbq+z9Rrj5XvPOVhJ8RMp3D5OoyF8l2QSH+gBco43v3L/fjrsAGzFesA=="),
                Role = Role.Personel
            }
        );
        }


    }
}
