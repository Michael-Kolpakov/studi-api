using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVideoStatusAndProcessingError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_ProcessingError_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ProcessingError",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "courses",
                table: "Videos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessingError",
                schema: "courses",
                table: "Videos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "courses",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_ProcessingError_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[ProcessingError] NOT LIKE '%[^A-Za-z0-9 .,]%'");
        }
    }
}
