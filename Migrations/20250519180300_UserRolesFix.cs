using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class UserRolesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Roles",
                table: "User_Roles");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "User_Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Roles",
                table: "User_Roles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_RoleName",
                table: "User_Roles",
                column: "RoleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Roles",
                table: "User_Roles");

            migrationBuilder.DropIndex(
                name: "IX_User_Roles_RoleName",
                table: "User_Roles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "User_Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Roles",
                table: "User_Roles",
                column: "RoleName");
        }
    }
}
