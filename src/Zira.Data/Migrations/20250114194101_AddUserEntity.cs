using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Reminders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Incomes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Expenses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Budgets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId1",
                table: "Reminders",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_UserId1",
                table: "Incomes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId1",
                table: "Expenses",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId1",
                table: "Budgets",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_AspNetUsers_UserId1",
                table: "Budgets",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_UserId1",
                table: "Expenses",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_AspNetUsers_UserId1",
                table: "Incomes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_AspNetUsers_UserId1",
                table: "Reminders",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_AspNetUsers_UserId1",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_UserId1",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_AspNetUsers_UserId1",
                table: "Incomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_AspNetUsers_UserId1",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_UserId1",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_UserId1",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_UserId1",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_UserId1",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");
        }
    }
}
