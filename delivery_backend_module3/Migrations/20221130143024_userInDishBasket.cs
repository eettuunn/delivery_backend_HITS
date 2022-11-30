using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery_backend_module3.Migrations
{
    public partial class userInDishBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishesInBasket_Basket_BasketId",
                table: "DishesInBasket");

            migrationBuilder.RenameColumn(
                name: "BasketId",
                table: "DishesInBasket",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DishesInBasket_BasketId",
                table: "DishesInBasket",
                newName: "IX_DishesInBasket_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "BasketEntityId",
                table: "DishesInBasket",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishesInBasket_BasketEntityId",
                table: "DishesInBasket",
                column: "BasketEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishesInBasket_Basket_BasketEntityId",
                table: "DishesInBasket",
                column: "BasketEntityId",
                principalTable: "Basket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishesInBasket_Users_UserId",
                table: "DishesInBasket",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishesInBasket_Basket_BasketEntityId",
                table: "DishesInBasket");

            migrationBuilder.DropForeignKey(
                name: "FK_DishesInBasket_Users_UserId",
                table: "DishesInBasket");

            migrationBuilder.DropIndex(
                name: "IX_DishesInBasket_BasketEntityId",
                table: "DishesInBasket");

            migrationBuilder.DropColumn(
                name: "BasketEntityId",
                table: "DishesInBasket");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DishesInBasket",
                newName: "BasketId");

            migrationBuilder.RenameIndex(
                name: "IX_DishesInBasket_UserId",
                table: "DishesInBasket",
                newName: "IX_DishesInBasket_BasketId");

            migrationBuilder.AddForeignKey(
                name: "FK_DishesInBasket_Basket_BasketId",
                table: "DishesInBasket",
                column: "BasketId",
                principalTable: "Basket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
