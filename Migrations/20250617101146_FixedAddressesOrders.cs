using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixedAddressesOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserAddressId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserAddressId",
                table: "Orders",
                column: "UserAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_UserAddressId",
                table: "Orders",
                column: "UserAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_UserAddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserAddressId",
                table: "Orders");
        }
    }
}
