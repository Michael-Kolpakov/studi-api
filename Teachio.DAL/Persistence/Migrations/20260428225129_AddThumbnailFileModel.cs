using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_ThumbnailName_Regex",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ThumbnailName",
                schema: "courses",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "ThumbnailFiles",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThumbnailName = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailFiles", x => x.Id);
                    table.CheckConstraint("CK_ThumbnailFile_ContentType_AllowedValues", "[ContentType] IN ('image/jpeg', 'image/png')");
                    table.CheckConstraint("CK_ThumbnailFile_Resolution_AspectRatioRange", "CHARINDEX('x', [Resolution]) > 1 AND CHARINDEX('x', [Resolution]) < LEN([Resolution]) AND CHARINDEX('x', [Resolution], CHARINDEX('x', [Resolution]) + 1) = 0 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) IS NOT NULL AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) IS NOT NULL AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) >= 1280 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) <= 2560 AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) >= 720 AND TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) <= 1440 AND TRY_CONVERT(int, LEFT([Resolution], CHARINDEX('x', [Resolution]) - 1)) * 9 = TRY_CONVERT(int, SUBSTRING([Resolution], CHARINDEX('x', [Resolution]) + 1, LEN([Resolution]))) * 16");
                    table.CheckConstraint("CK_ThumbnailFile_ThumbnailName_Regex", "[ThumbnailName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");
                    table.ForeignKey(
                        name: "FK_ThumbnailFiles_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThumbnailFiles_CourseId",
                schema: "courses",
                table: "ThumbnailFiles",
                column: "CourseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThumbnailFiles",
                schema: "courses");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailName",
                schema: "courses",
                table: "Courses",
                type: "nvarchar(110)",
                maxLength: 110,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_ThumbnailName_Regex",
                schema: "courses",
                table: "Courses",
                sql: "[ThumbnailName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");
        }
    }
}
