using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThresholdForBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WarningThreshold",
                table: "Budgets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarningThreshold",
                table: "Budgets");
        }
    }
}
