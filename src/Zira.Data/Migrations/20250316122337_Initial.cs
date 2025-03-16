using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zira.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredCurrencyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Currencies_PreferredCurrencyCode",
                        column: x => x.PreferredCurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ToCurrencyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_FromCurrencyCode",
                        column: x => x.FromCurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code");
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_ToCurrencyCode",
                        column: x => x.ToCurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WarningThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpentPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Month = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReminderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnableBillReminders = table.Column<bool>(type: "bit", nullable: false),
                    EnableBudgetAlerts = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SavingsGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavingsGoals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    Recurrence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code");
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name", "Symbol" },
                values: new object[,]
                {
                    { "AUD", "Australian Dollar", "A$" },
                    { "BGN", "Bulgarian Lev", "лв." },
                    { "CAD", "Canadian Dollar", "C$" },
                    { "EUR", "Euro", "€" },
                    { "GBP", "British Pound", "£" },
                    { "JPY", "Japanese Yen", "¥" },
                    { "USD", "US Dollar", "$" }
                });

            migrationBuilder.InsertData(
                table: "ExchangeRates",
                columns: new[] { "Id", "FromCurrencyCode", "LastUpdated", "Rate", "ToCurrencyCode" },
                values: new object[,]
                {
                    { 1, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5713), 0.5539m, "USD" },
                    { 2, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5717), 0.5094m, "EUR" },
                    { 3, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5719), 0.4276m, "GBP" },
                    { 4, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5721), 74.76m, "JPY" },
                    { 5, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5722), 0.7456m, "CAD" },
                    { 6, "BGN", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5725), 0.8284m, "AUD" },
                    { 7, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5808), 1.805380032496840584943130529m, "BGN" },
                    { 8, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5811), 1.9630938358853553199842952493m, "BGN" },
                    { 9, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5832), 2.3386342376052385406922357343m, "BGN" },
                    { 10, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5833), 0.01337613697164258962011771m, "BGN" },
                    { 11, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5835), 1.3412017167381974248927038627m, "BGN" },
                    { 12, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(5838), 1.2071463061323032351521004346m, "BGN" },
                    { 13, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6140), 0.9196605885538905939700306915m, "EUR" },
                    { 14, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6146), 1.0873576756968983117393011385m, "USD" },
                    { 15, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6150), 0.7719805018956490341216826142m, "GBP" },
                    { 16, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6154), 1.2953695042095416276894293732m, "USD" },
                    { 17, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6158), 134.97021122946380213034843835m, "JPY" },
                    { 18, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6162), 0.0074090422685928303905831996m, "USD" },
                    { 19, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6166), 1.3460913522296443401335981224m, "CAD" },
                    { 20, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6169), 0.7428916309012875536480686695m, "USD" },
                    { 21, "USD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6173), 1.4955768189203827405668893302m, "AUD" },
                    { 22, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6177), 0.6686383389666827619507484307m, "USD" },
                    { 23, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6181), 0.8394189242245779348252846486m, "GBP" },
                    { 24, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6184), 1.1913002806361085126286248831m, "EUR" },
                    { 25, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6188), 146.76089517078916372202591284m, "JPY" },
                    { 26, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6190), 0.0068138041733547351524879615m, "EUR" },
                    { 27, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6195), 1.4636827640361209265802905379m, "CAD" },
                    { 28, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6197), 0.6832081545064377682403433476m, "EUR" },
                    { 29, "EUR", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6202), 1.6262269336474283470749901845m, "AUD" },
                    { 30, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6205), 0.6149203283437952679864799614m, "EUR" },
                    { 31, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6209), 174.8362956033676333021515435m, "JPY" },
                    { 32, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6211), 0.0057196361690743713215623328m, "GBP" },
                    { 33, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6216), 1.7436856875584658559401309635m, "CAD" },
                    { 34, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6218), 0.5734978540772532188841201717m, "GBP" },
                    { 35, "GBP", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6223), 1.9373246024321796071094480823m, "AUD" },
                    { 36, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6225), 0.5161757605021728633510381458m, "GBP" },
                    { 37, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6229), 0.0099732477260567148207597646m, "CAD" },
                    { 38, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6232), 100.26824034334763948497854057m, "JPY" },
                    { 39, "JPY", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6280), 0.011080791867308721241305511m, "AUD" },
                    { 40, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6283), 90.24625784645098985997102823m, "JPY" },
                    { 41, "CAD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6289), 1.1110515021459227467811158798m, "AUD" },
                    { 42, "AUD", new DateTime(2025, 3, 16, 12, 23, 37, 436, DateTimeKind.Utc).AddTicks(6291), 0.900048285852245292129406084m, "CAD" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PreferredCurrencyCode",
                table: "AspNetUsers",
                column: "PreferredCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrencyCode",
                table: "ExchangeRates",
                column: "FromCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ToCurrencyCode",
                table: "ExchangeRates",
                column: "ToCurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderSettings_UserId",
                table: "ReminderSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_UserId",
                table: "SavingsGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CurrencyCode",
                table: "Transactions",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "ReminderSettings");

            migrationBuilder.DropTable(
                name: "SavingsGoals");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
