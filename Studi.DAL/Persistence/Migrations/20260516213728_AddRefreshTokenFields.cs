using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenCreatedAt",
                schema: "users",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                schema: "users",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                schema: "users",
                table: "AppUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenRevokedAt",
                schema: "users",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_RefreshTokenHash",
                schema: "users",
                table: "AppUsers",
                column: "RefreshTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppUser_RefreshTokenHash",
                schema: "users",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenCreatedAt",
                schema: "users",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                schema: "users",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                schema: "users",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenRevokedAt",
                schema: "users",
                table: "AppUsers");
        }
    }
}
