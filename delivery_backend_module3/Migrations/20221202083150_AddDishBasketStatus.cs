using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery_backend_module3.Migrations
{
    public partial class AddDishBasketStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropTable(
                name: "DishBasketDto");*/

            migrationBuilder.AddColumn<int>(
                name: "DishStatus",
                table: "DishesInBasket",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderEntityId",
                table: "DishesInBasket",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishesInBasket_OrderEntityId",
                table: "DishesInBasket",
                column: "OrderEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishesInBasket_Orders_OrderEntityId",
                table: "DishesInBasket",
                column: "OrderEntityId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishesInBasket_Orders_OrderEntityId",
                table: "DishesInBasket");

            migrationBuilder.DropIndex(
                name: "IX_DishesInBasket_OrderEntityId",
                table: "DishesInBasket");

            migrationBuilder.DropColumn(
                name: "DishStatus",
                table: "DishesInBasket");

            migrationBuilder.DropColumn(
                name: "OrderEntityId",
                table: "DishesInBasket");

            migrationBuilder.CreateTable(
                name: "DishBasketDto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    image = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    totalPrice = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishBasketDto", x => x.id);
                    table.ForeignKey(
                        name: "FK_DishBasketDto_Orders_OrderEntityId",
                        column: x => x.OrderEntityId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishBasketDto_OrderEntityId",
                table: "DishBasketDto",
                column: "OrderEntityId");
        }
    }
}
