using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIHarmony.Migrations
{
    public partial class MigrationForConfidentialWords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfidentialWords",
                columns: table => new
                {
                    ConfidentialWordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfidentialWords", x => x.ConfidentialWordId);
                    table.ForeignKey(
                        name: "FK_ConfidentialWords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "usedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfidentialWords_UserId",
                table: "ConfidentialWords",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfidentialWords");
        }
    }
}
