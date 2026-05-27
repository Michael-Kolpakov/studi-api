using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WatchingUsersCountField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WatchingUsersCount",
                schema: "courses",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_WatchingUsersCount_NonNegative",
                schema: "courses",
                table: "Courses",
                sql: "[WatchingUsersCount] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_WatchingUsersCount_NonNegative",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "WatchingUsersCount",
                schema: "courses",
                table: "Courses");
        }
    }
}
