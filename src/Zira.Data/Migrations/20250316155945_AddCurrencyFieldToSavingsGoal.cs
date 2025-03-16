using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFieldToSavingsGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "SavingsGoals",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_CurrencyCode",
                table: "SavingsGoals",
                column: "CurrencyCode");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingsGoals_Currencies_CurrencyCode",
                table: "SavingsGoals",
                column: "CurrencyCode",
                principalTable: "Currencies",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingsGoals_Currencies_CurrencyCode",
                table: "SavingsGoals");

            migrationBuilder.DropIndex(
                name: "IX_SavingsGoals_CurrencyCode",
                table: "SavingsGoals");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "SavingsGoals");

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5713));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5717));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5719));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5721));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5722));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5725));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5808));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5811));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5832));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5833));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5835));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5838));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 13,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6140));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 14,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6146));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 15,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6150));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 16,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6154));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 17,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6158));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 18,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6162));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 19,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6166));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 20,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6169));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 21,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6173));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 22,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6177));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 23,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6181));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 24,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6184));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 25,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6188));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 26,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6190));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 27,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6195));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 28,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6197));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 29,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6202));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 30,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6205));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 31,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 32,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6211));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 33,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6216));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 34,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6218));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 35,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6223));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 36,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6225));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 37,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6229));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 38,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6232));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 39,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6280));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 40,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6283));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 41,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6289));

            migrationBuilder.UpdateData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 42,
                column: "LastUpdated",
                value: new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6291));
        }
    }
}
