using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "courses",
                table: "Videos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_Description_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[Description] NOT LIKE '%[^A-Za-z0-9 ,.!?%$#\"'':&()+=*/-–]%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_Description_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "courses",
                table: "Videos");
        }
    }
}
