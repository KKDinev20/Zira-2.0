using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionSetAsideFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSetAside",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5746));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5751));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5753));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5755));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5757));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5761));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5862));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5866));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5868));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5870));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5872));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(5876));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6457));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6466));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6473));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6477));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6488));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6494));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6497));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6508));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6513));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6517));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6523));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6526));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6532));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6536));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6610));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6615));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6624));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6627));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6634));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6638));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6645));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6648));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6655));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6659));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6667));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6671));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6679));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2026, 11, 8, 17, 49, 54, 888, DateTimeKind.Utc).AddTicks(6683));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSetAside",
                table: "Transactions");

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2671));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2676));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2678));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2680));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2682));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2685));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2786));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2792));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2795));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2799));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2803));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(2808));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3449));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3465));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3474));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3490));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3498));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3507));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3513));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3522));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3529));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3538));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3544));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3553));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3558));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3568));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3573));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3583));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3590));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3600));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3606));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3615));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3620));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3630));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3635));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3645));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3650));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3660));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3666));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3677));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2025, 4, 13, 14, 34, 52, 891, DateTimeKind.Utc).AddTicks(3682));
        }
    }
}
