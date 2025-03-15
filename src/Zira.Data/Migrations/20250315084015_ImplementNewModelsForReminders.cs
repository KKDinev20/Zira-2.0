using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImplementNewModelsForReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Reminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotified",
                table: "Reminders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Reminders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NotificationMethod",
                table: "Reminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReminderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnableBillReminders = table.Column<bool>(type: "bit", nullable: false),
                    EnableBudgetAlerts = table.Column<bool>(type: "bit", nullable: false),
                    PreferredNotification = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReminderSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReminderSettings_UserId",
                table: "ReminderSettings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReminderSettings");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsNotified",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "NotificationMethod",
                table: "Reminders");
        }
    }
}
