using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueVideoOrderIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_SectionId",
                schema: "courses",
                table: "Videos");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_SectionId_OrderIndex",
                schema: "courses",
                table: "Videos",
                columns: new[] { "SectionId", "OrderIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_SectionId_OrderIndex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_SectionId",
                schema: "courses",
                table: "Videos",
                column: "SectionId");
        }
    }
}
