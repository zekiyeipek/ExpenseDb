using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseDb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseCategoryWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseCategoryId",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ulaşım" },
                    { 2, "Yemek" },
                    { 3, "Konaklama" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
