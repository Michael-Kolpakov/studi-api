using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studi.DAL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingRegistrations",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PinHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PinExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PinSentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRegistrations", x => x.Id);
                    table.CheckConstraint("CK_PendingRegistration_Name_Regex", "[Name] NOT LIKE '%[^A-Za-z'' ]%'");
                    table.CheckConstraint("CK_PendingRegistration_Surname_Regex", "[Surname] NOT LIKE '%[^A-Za-z'' ]%'");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingRegistration_NormalizedEmail",
                schema: "users",
                table: "PendingRegistrations",
                column: "NormalizedEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRegistrations",
                schema: "users");
        }
    }
}
