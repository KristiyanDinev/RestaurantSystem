using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "LastTimeLogedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 18),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2025, 7, 15));

            migrationBuilder.AddColumn<string>(
                name: "CuponCode",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CuponCode",
                table: "Orders",
                column: "CuponCode");

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

            migrationBuilder.DropColumn(
                name: "CuponCode",
                table: "Orders");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LastTimeLogedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 15),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2025, 7, 18));
        }
    }
}
