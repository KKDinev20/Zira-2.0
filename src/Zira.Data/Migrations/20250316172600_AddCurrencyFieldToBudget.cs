using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Budgets",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CurrencyCode",
                table: "Budgets",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Currencies_CurrencyCode",
                table: "Budgets",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Currencies_CurrencyCode",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_CurrencyCode",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Budgets");

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(8963));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(8965));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(8967));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(8968));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(8970));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9001));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9099));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9101));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9103));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9104));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9105));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9108));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9407));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9428));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9432));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9435));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9439));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9443));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9447));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9449));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9453));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9456));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9460));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9463));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9467));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9469));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9473));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9476));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9480));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9483));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9487));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9489));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9493));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9495));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9499));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9501));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9505));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9507));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9536));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9539));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9543));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 15, 59, 45, 227, DateTimeKind.Utc).AddTicks(9546));
        }
    }
}
