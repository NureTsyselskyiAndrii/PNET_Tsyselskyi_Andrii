using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DEPT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    INFO = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DEPT_ID);
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    GOOD_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PRICE = table.Column<double>(type: "float", nullable: true),
                    QUANTITY = table.Column<int>(type: "int", nullable: true),
                    PRODUCER = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DEPT_ID = table.Column<int>(type: "int", nullable: true),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.GOOD_ID);
                    table.ForeignKey(
                        name: "FK_Goods_Departments_DEPT_ID",
                        column: x => x.DEPT_ID,
                        principalTable: "Departments",
                        principalColumn: "DEPT_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    SALE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CHECK_NO = table.Column<int>(type: "int", nullable: false),
                    GOOD_ID = table.Column<int>(type: "int", nullable: false),
                    DATE_SALE = table.Column<DateTime>(type: "date", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.SALE_ID);
                    table.ForeignKey(
                        name: "FK_Sales_Goods_GOOD_ID",
                        column: x => x.GOOD_ID,
                        principalTable: "Goods",
                        principalColumn: "GOOD_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DEPT_ID", "INFO", "NAME" },
                values: new object[,]
                {
                    { 1, "Харчові товари", "Продукти" },
                    { 2, "Техніка та гаджети", "Електроніка" },
                    { 3, "Чоловічий та жіночий", "Одяг" },
                    { 4, "Засоби для дому", "Побутхімія" },
                    { 5, "Офісні та шкільні", "Канцтовари" }
                });

            migrationBuilder.InsertData(
                table: "Goods",
                columns: new[] { "GOOD_ID", "DEPT_ID", "DESCRIPTION", "NAME", "PRICE", "PRODUCER", "QUANTITY" },
                values: new object[,]
                {
                    { 1, 1, "Молоко 2.5%, 1л", "Молоко", 28.5, "Галичина", 120 },
                    { 2, 1, "Житній хліб 400г", "Хліб чорний", 22.0, "Київхліб", 80 },
                    { 3, 2, "Бездротові BT 5.0", "Навушники", 850.0, "Samsung", 15 },
                    { 4, 3, "Бавовна 100%, XL", "Футболка", 299.0, "H&M", 40 },
                    { 5, 4, "Автомат 1.5 кг", "Порошок", 145.0, "Tide", 55 },
                    { 6, 5, "96 аркушів, клітинка", "Зошит", 35.0, "Buromax", 200 }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SALE_ID", "CHECK_NO", "DATE_SALE", "GOOD_ID", "QUANTITY" },
                values: new object[,]
                {
                    { 1, 1001, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3 },
                    { 2, 1001, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                    { 3, 1002, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                    { 4, 1003, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2 },
                    { 5, 1003, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1 },
                    { 6, 1004, new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goods_DEPT_ID",
                table: "Goods",
                column: "DEPT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_GOOD_ID",
                table: "Sales",
                column: "GOOD_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
