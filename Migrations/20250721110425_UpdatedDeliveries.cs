using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Delivery_UserId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "LastTimeLogedIn",
                table: "Users");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastTimeLoggedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 21));

            migrationBuilder.AddColumn<string>(
                name: "CuponCode",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CuponCode",
                table: "Orders",
                column: "CuponCode");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_UserId",
                table: "Delivery",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Cupons_CuponCode",
                table: "Orders",
                column: "CuponCode",
                principalTable: "Cupons",
                principalColumn: "CuponCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cupons_CuponCode",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CuponCode",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_UserId",
                table: "Delivery");

            migrationBuilder.DropColumn(
                name: "LastTimeLoggedIn",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CuponCode",
                table: "Orders");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastTimeLogedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 15));

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_UserId",
                table: "Delivery",
                column: "UserId",
                unique: true);
        }
    }
}
