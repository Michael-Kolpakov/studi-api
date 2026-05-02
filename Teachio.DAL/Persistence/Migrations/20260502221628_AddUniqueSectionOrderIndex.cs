using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teachio.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueSectionOrderIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId",
                schema: "courses",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_OrderIndex",
                schema: "courses",
                table: "Sections",
                columns: new[] { "CourseId", "OrderIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId_OrderIndex",
                schema: "courses",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                schema: "courses",
                table: "Sections",
                column: "CourseId");
        }
    }
}
