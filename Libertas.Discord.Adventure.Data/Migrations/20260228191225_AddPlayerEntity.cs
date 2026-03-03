using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libertas.Discord.Adventure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AttackLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    AttackXp = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    MagicLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    MagicXp = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    SpeechLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    SpeechXp = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    DefenseLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    DefenseXp = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    TotalGold = table.Column<double>(type: "REAL", nullable: false, defaultValue: 0.0),
                    TotalKills = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    TotalDeaths = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    HighestDungeonLevel = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastActiveAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_HighestDungeonLevel",
                table: "Players",
                column: "HighestDungeonLevel",
                descending: []);

            migrationBuilder.CreateIndex(
                name: "IX_Players_TotalGold",
                table: "Players",
                column: "TotalGold",
                descending: []);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
