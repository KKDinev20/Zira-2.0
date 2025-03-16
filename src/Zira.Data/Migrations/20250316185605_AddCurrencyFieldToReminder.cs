using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldToReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Reminders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1527));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1532));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1534));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1536));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1538));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1541));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1701));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1705));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1707));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1709));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1711));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(1715));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2166));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2175));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2182));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2186));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2212));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2216));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2224));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2228));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2236));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2241));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2248));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2251));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2259));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2262));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2269));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2273));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2281));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2286));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2294));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2336));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2345));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2348));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2357));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2368));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2372));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2384));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2393));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 18, 56, 4, 893, DateTimeKind.Utc).AddTicks(2397));

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_CurrencyCode",
                table: "Reminders",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Currencies_CurrencyCode",
                table: "Reminders",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Currencies_CurrencyCode",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_CurrencyCode",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Reminders");

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2506));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2509));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2511));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2512));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2514));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2517));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2605));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2608));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2609));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2611));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2612));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2615));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2930));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2964));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2969));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2973));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2977));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2981));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2986));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2989));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2994));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(2998));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3003));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3006));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3011));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3013));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3018));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3020));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3025));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3029));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3034));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3036));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3044));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3046));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3052));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3054));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3060));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3063));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3068));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3071));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3077));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 17, 25, 59, 899, DateTimeKind.Utc).AddTicks(3080));
        }
    }
}
