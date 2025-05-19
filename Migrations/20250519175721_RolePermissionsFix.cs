using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class RolePermissionsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Permissions_Roles_RoleName",
                table: "Role_Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role_Permissions",
                table: "Role_Permissions");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Role_Permissions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Role_Permissions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role_Permissions",
                table: "Role_Permissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Permissions_ServicePath",
                table: "Role_Permissions",
                column: "ServicePath");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Permissions_Roles_RoleName",
                table: "Role_Permissions",
                column: "RoleName",
                principalTable: "Roles",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Permissions_Roles_RoleName",
                table: "Role_Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role_Permissions",
                table: "Role_Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Role_Permissions_ServicePath",
                table: "Role_Permissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Role_Permissions");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Role_Permissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role_Permissions",
                table: "Role_Permissions",
                column: "ServicePath");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Permissions_Roles_RoleName",
                table: "Role_Permissions",
                column: "RoleName",
                principalTable: "Roles",
                principalColumn: "Name");
        }
    }
}
