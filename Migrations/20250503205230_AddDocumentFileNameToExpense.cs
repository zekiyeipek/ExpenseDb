using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseDb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFileNameToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentFileName",
                table: "Expenses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentFileName",
                table: "Expenses");
        }
    }
}
