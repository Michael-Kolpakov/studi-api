using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVideoProgressPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VideoProgress_VideoId",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                schema: "courses",
                table: "VideoProgress",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_AppUserId",
                schema: "courses",
                table: "VideoProgress",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_VideoId_AppUserId",
                schema: "courses",
                table: "VideoProgress",
                columns: new[] { "VideoId", "AppUserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoProgress_AppUsers_AppUserId",
                schema: "courses",
                table: "VideoProgress",
                column: "AppUserId",
                principalSchema: "users",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoProgress_AppUsers_AppUserId",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.DropIndex(
                name: "IX_VideoProgress_AppUserId",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.DropIndex(
                name: "IX_VideoProgress_VideoId_AppUserId",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProgress_VideoId",
                schema: "courses",
                table: "VideoProgress",
                column: "VideoId",
                unique: true);
        }
    }
}
