using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStringRegexAndEditRangesChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_OrderIndex_NonNegative",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_PositionSeconds_Range",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_OrderIndex_NonNegative",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_VideosCount_Max",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_SectionsCount_Max",
                schema: "courses",
                table: "Courses");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_OrderIndex_Range",
                schema: "courses",
                table: "Videos",
                sql: "[OrderIndex] >= 0 AND [OrderIndex] <= 39");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_ProcessingError_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[ProcessingError] NOT LIKE '%[^A-Za-z0-9 .,]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_Title_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[Title] NOT LIKE '%[^A-Za-z0-9 ,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_VideoName_Regex",
                schema: "courses",
                table: "Videos",
                sql: "[VideoName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_VideoProgress_PositionSeconds_Range",
                schema: "courses",
                table: "VideoProgress",
                sql: "[PositionSeconds] >= 0 AND [PositionSeconds] <= 3600");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_OrderIndex_Range",
                schema: "courses",
                table: "Sections",
                sql: "[OrderIndex] >= 0 AND [OrderIndex] <= 34");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_SectionName_Regex",
                schema: "courses",
                table: "Sections",
                sql: "[SectionName] NOT LIKE '%[^A-Za-z0-9,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_Title_Regex",
                schema: "courses",
                table: "Sections",
                sql: "[Title] NOT LIKE '%[^A-Za-z0-9 ,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_VideosCount_Range",
                schema: "courses",
                table: "Sections",
                sql: "[VideosCount] >= 0 AND [VideosCount] <= 40");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_CourseName_Regex",
                schema: "courses",
                table: "Courses",
                sql: "[CourseName] NOT LIKE '%[^A-Za-z0-9,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_Description_Regex",
                schema: "courses",
                table: "Courses",
                sql: "[Description] NOT LIKE '%[^A-Za-z0-9 ,.!?%$#\"'':&()+=*/-–]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_SectionsCount_Range",
                schema: "courses",
                table: "Courses",
                sql: "[SectionsCount] >= 0 AND [SectionsCount] <= 35");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_ThumbnailName_Regex",
                schema: "courses",
                table: "Courses",
                sql: "[ThumbnailName] NOT LIKE '%[^A-Za-z0-9.,!?-]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_Title_Regex",
                schema: "courses",
                table: "Courses",
                sql: "[Title] NOT LIKE '%[^A-Za-z0-9 ,!?-]%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_OrderIndex_Range",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_ProcessingError_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_Title_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Video_VideoName_Regex",
                schema: "courses",
                table: "Videos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VideoProgress_PositionSeconds_Range",
                schema: "courses",
                table: "VideoProgress");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_OrderIndex_Range",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_SectionName_Regex",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_Title_Regex",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Section_VideosCount_Range",
                schema: "courses",
                table: "Sections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_CourseName_Regex",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_Description_Regex",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_SectionsCount_Range",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_ThumbnailName_Regex",
                schema: "courses",
                table: "Courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Course_Title_Regex",
                schema: "courses",
                table: "Courses");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_OrderIndex_NonNegative",
                schema: "courses",
                table: "Videos",
                sql: "[OrderIndex] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Video_PositionSeconds_Range",
                schema: "courses",
                table: "VideoProgress",
                sql: "[PositionSeconds] >= 0 AND [PositionSeconds] <= 3600");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_OrderIndex_NonNegative",
                schema: "courses",
                table: "Sections",
                sql: "[OrderIndex] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Section_VideosCount_Max",
                schema: "courses",
                table: "Sections",
                sql: "[VideosCount] >= 0 AND [VideosCount] <= 40");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Course_SectionsCount_Max",
                schema: "courses",
                table: "Courses",
                sql: "[SectionsCount] >= 0 AND [SectionsCount] <= 35");
        }
    }
}
