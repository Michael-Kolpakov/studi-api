using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoResolutionMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Resolution",
                schema: "courses",
                table: "VideoFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_VideoFile_Resolution_AllowedValues",
                schema: "courses",
                table: "VideoFiles",
                sql: "[Resolution] IN (480, 720, 1080, 1440)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_VideoFile_Resolution_AllowedValues",
                schema: "courses",
                table: "VideoFiles");

            migrationBuilder.DropColumn(
                name: "Resolution",
                schema: "courses",
                table: "VideoFiles");
        }
    }
}
