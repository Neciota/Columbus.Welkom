using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Columbus.Welkom.Application.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "owner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Club = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "race",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_race", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "league",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    League = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league", x => x.Id);
                    table.ForeignKey(
                        name: "FK_league_owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    RingNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Chip = table.Column<int>(type: "INTEGER", nullable: false),
                    Sex = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pigeon_owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstOwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondOwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ThirdOwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_team_owner_FirstOwnerId",
                        column: x => x.FirstOwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_owner_SecondOwnerId",
                        column: x => x.SecondOwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_owner_ThirdOwnerId",
                        column: x => x.ThirdOwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon_race",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PigeonId = table.Column<int>(type: "INTEGER", nullable: false),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mark = table.Column<int>(type: "INTEGER", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon_race", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pigeon_race_pigeon_PigeonId",
                        column: x => x.PigeonId,
                        principalTable: "pigeon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_race_race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon_swap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PigeonId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoupledPlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon_swap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_CoupledPlayerId",
                        column: x => x.CoupledPlayerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_pigeon_PigeonId",
                        column: x => x.PigeonId,
                        principalTable: "pigeon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selected_year_pigeon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PigeonId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selected_year_pigeon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_selected_year_pigeon_owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_selected_year_pigeon_pigeon_PigeonId",
                        column: x => x.PigeonId,
                        principalTable: "pigeon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selected_young_pigeon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PigeonId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selected_young_pigeon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_selected_young_pigeon_owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "owner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_selected_young_pigeon_pigeon_PigeonId",
                        column: x => x.PigeonId,
                        principalTable: "pigeon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_league_OwnerId",
                table: "league",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_OwnerId",
                table: "pigeon",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_RingNumber",
                table: "pigeon",
                column: "RingNumber");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_Year",
                table: "pigeon",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_race_Mark",
                table: "pigeon_race",
                column: "Mark");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_race_PigeonId",
                table: "pigeon_race",
                column: "PigeonId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_race_RaceId",
                table: "pigeon_race",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_CoupledPlayerId",
                table: "pigeon_swap",
                column: "CoupledPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_OwnerId",
                table: "pigeon_swap",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_PigeonId",
                table: "pigeon_swap",
                column: "PigeonId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_PlayerId",
                table: "pigeon_swap",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_Year",
                table: "pigeon_swap",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_race_Code",
                table: "race",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_race_Number",
                table: "race",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_selected_year_pigeon_OwnerId",
                table: "selected_year_pigeon",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_selected_year_pigeon_PigeonId",
                table: "selected_year_pigeon",
                column: "PigeonId");

            migrationBuilder.CreateIndex(
                name: "IX_selected_young_pigeon_OwnerId",
                table: "selected_young_pigeon",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_selected_young_pigeon_PigeonId",
                table: "selected_young_pigeon",
                column: "PigeonId");

            migrationBuilder.CreateIndex(
                name: "IX_team_FirstOwnerId",
                table: "team",
                column: "FirstOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_team_SecondOwnerId",
                table: "team",
                column: "SecondOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_team_ThirdOwnerId",
                table: "team",
                column: "ThirdOwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "league");

            migrationBuilder.DropTable(
                name: "pigeon_race");

            migrationBuilder.DropTable(
                name: "pigeon_swap");

            migrationBuilder.DropTable(
                name: "selected_year_pigeon");

            migrationBuilder.DropTable(
                name: "selected_young_pigeon");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropTable(
                name: "race");

            migrationBuilder.DropTable(
                name: "pigeon");

            migrationBuilder.DropTable(
                name: "owner");
        }
    }
}
