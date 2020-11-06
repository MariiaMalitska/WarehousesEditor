using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WarehousesEditor.Migrations
{
    public partial class DbScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Code = table.Column<string>(unicode: false, maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "money", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarehouseName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Address = table.Column<string>(unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseId);
                });

            migrationBuilder.Sql(
            @"CREATE FUNCTION [dbo].[ComputePrice](@CurrencyId INT, @BaseCurrencyPrice MONEY)  
            RETURNS MONEY  
            AS  
            BEGIN  
             DECLARE @Price MONEY  
             DECLARE @Rate MONEY
            SELECT @Rate = Rate FROM Currencies WHERE CurrencyId = @CurrencyId
            SET @Price = @Rate * @BaseCurrencyPrice  
            RETURN  
            @Price  
            END");

        migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    GoodsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsName = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    BaseCurrencyPrice = table.Column<decimal>(type: "money", nullable: false),
                    CurrencyId = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    Price = table.Column<decimal>(type: "money", nullable: true, computedColumnSql: "([dbo].[ComputePrice]([CurrencyId],[BaseCurrencyPrice]))"),
                    BarcodeNumber = table.Column<string>(unicode: false, maxLength: 8, nullable: false, defaultValueSql: "('00000000')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.GoodsId);
                    table.ForeignKey(
                        name: "FK_Goods_Currencies",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoodsCategories",
                columns: table => new
                {
                    GoodsId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsCategories", x => new { x.GoodsId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_GoodsCategories_Categories",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsCategories_Goods",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "GoodsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehousesGoods",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(nullable: false),
                    GoodsId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehousesGoods", x => new { x.WarehouseId, x.GoodsId });
                    table.ForeignKey(
                        name: "FK_WarehousesGoods_Goods",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "GoodsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehousesGoods_Warehouses",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyName",
                table: "Currencies",
                column: "CurrencyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goods_BarcodeNumber",
                table: "Goods",
                column: "BarcodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goods_CurrencyId",
                table: "Goods",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_GoodsName",
                table: "Goods",
                column: "GoodsName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsCategories_CategoryId",
                table: "GoodsCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_Address",
                table: "Warehouses",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehousesGoods_GoodsId",
                table: "WarehousesGoods",
                column: "GoodsId");

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION [dbo].[ComputePrice]");

            migrationBuilder.DropTable(
                name: "GoodsCategories");

            migrationBuilder.DropTable(
                name: "WarehousesGoods");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
