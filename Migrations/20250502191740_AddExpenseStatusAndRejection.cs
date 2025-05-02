using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseDb.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseStatusAndRejection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Expenses");
        }
    }
}
