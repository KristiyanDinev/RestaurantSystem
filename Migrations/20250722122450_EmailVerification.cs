using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RestaurantSystem.Migrations
{
    /// <inheritdoc />
    public partial class EmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "LastTimeLoggedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 22),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2025, 7, 21));

            migrationBuilder.CreateTable(
                name: "Email_Verification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email_Verification", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Email_Verification_Code",
                table: "Email_Verification",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Email_Verification_Email",
                table: "Email_Verification",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Email_Verification");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "LastTimeLoggedIn",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 7, 21),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2025, 7, 22));
        }
    }
}
