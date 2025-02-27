using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipTransactionSavings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SavingsGoalId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SavingsGoalId",
                table: "Transactions",
                column: "SavingsGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SavingsGoals_SavingsGoalId",
                table: "Transactions",
                column: "SavingsGoalId",
                principalTable: "SavingsGoals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SavingsGoals_SavingsGoalId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SavingsGoalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SavingsGoalId",
                table: "Transactions");
        }
    }
}
