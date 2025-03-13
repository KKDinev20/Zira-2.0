using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");
        }
    }
}
