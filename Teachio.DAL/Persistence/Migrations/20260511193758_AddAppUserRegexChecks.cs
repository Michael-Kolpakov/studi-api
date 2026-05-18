using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserRegexChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_AppUser_Name_Regex",
                schema: "users",
                table: "AppUsers",
                sql: "[Name] NOT LIKE '%[^A-Za-z'' ]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AppUser_Surname_Regex",
                schema: "users",
                table: "AppUsers",
                sql: "[Surname] NOT LIKE '%[^A-Za-z'' ]%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_AppUser_Name_Regex",
                schema: "users",
                table: "AppUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AppUser_Surname_Regex",
                schema: "users",
                table: "AppUsers");
        }
    }
}
