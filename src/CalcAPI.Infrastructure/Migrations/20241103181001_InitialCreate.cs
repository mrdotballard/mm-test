using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalcAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Operation = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InputValue1 = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    InputValue2 = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Result = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRequests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogRequests");
        }
    }
}
