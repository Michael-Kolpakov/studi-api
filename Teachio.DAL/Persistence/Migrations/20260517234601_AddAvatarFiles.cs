using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvatarFiles",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvatarName = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarFiles", x => x.Id);
                    table.CheckConstraint("CK_AvatarFile_AvatarName_Regex", "[AvatarName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");
                    table.CheckConstraint("CK_AvatarFile_ContentType_AllowedValues", "[ContentType] IN ('image/jpeg', 'image/png')");
                    table.CheckConstraint("CK_AvatarFile_Resolution_AspectRatioRange", "CHARINDEX('x', [Resolution]) > 1 AND CHARINDEX('x', [Resolution]) < LEN([Resolution]) AND CHARINDEX('x', [Resolution], CHARINDEX('x', [Resolution]) + 1) = 0 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) IS NOT NULL AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) IS NOT NULL AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) >= 480 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) <= 1440 AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) >= 480 AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) <= 1440 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) * 1 = TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) * 1");
                    table.ForeignKey(
                        name: "FK_AvatarFiles_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalSchema: "users",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvatarFiles_AppUserId",
                schema: "users",
                table: "AvatarFiles",
                column: "AppUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvatarFiles",
                schema: "users");
        }
    }
}
