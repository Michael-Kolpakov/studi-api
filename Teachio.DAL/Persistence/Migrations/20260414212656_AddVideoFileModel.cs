using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoFileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_DurationSeconds_Range",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_VideoName_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoName",
                schema: "courses",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "VideoFiles",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VideoName = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoFiles", x => x.Id);
                    table.CheckConstraint("CK_VideoFile_ContentType_AllowedValues", "[ContentType] IN ('video/mp4', 'video/quicktime')");
                    table.CheckConstraint("CK_VideoFile_DurationSeconds_Range", "[DurationSeconds] >= 0 AND [DurationSeconds] <= 3600");
                    table.CheckConstraint("CK_VideoFile_VideoName_Regex", "[VideoName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");
                    table.ForeignKey(
                        name: "FK_VideoFiles_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "courses",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoFiles_VideoId",
                schema: "courses",
                table: "VideoFiles",
                column: "VideoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoFiles",
                schema: "courses");

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                schema: "courses",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VideoName",
                schema: "courses",
                table: "Videos",
                type: "nvarchar(110)",
                maxLength: 110,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_DurationSeconds_Range",
                schema: "courses",
                table: "Videos",
                sql: "[DurationSeconds] >= 0 AND [DurationSeconds] <= 3600");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_VideoName_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[VideoName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");
        }
    }
}
