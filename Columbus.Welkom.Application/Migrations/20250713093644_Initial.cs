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
                name: "league",
                columns: table => new
                {
                    rank = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league", x => x.rank);
                });

            migrationBuilder.CreateTable(
                name: "owner",
                columns: table => new
                {
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    latitude = table.Column<double>(type: "REAL", nullable: false),
                    longitude = table.Column<double>(type: "REAL", nullable: false),
                    club_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owner", x => x.owner_id);
                });

            migrationBuilder.CreateTable(
                name: "race",
                columns: table => new
                {
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    number = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    start_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    latitude = table.Column<double>(type: "REAL", nullable: false),
                    longitude = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_race", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    number = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team", x => x.number);
                });

            migrationBuilder.CreateTable(
                name: "league_owner",
                columns: table => new
                {
                    league_rank = table.Column<int>(type: "INTEGER", nullable: false),
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_owner", x => new { x.owner_id, x.league_rank });
                    table.ForeignKey(
                        name: "FK_league_owner_league_league_rank",
                        column: x => x.league_rank,
                        principalTable: "league",
                        principalColumn: "rank",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_league_owner_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon",
                columns: table => new
                {
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false),
                    chip = table.Column<int>(type: "INTEGER", nullable: false),
                    sex = table.Column<int>(type: "INTEGER", nullable: false),
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon", x => new { x.country_code, x.year, x.ring_number });
                    table.ForeignKey(
                        name: "FK_pigeon_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "owner_team",
                columns: table => new
                {
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false),
                    team_number = table.Column<int>(type: "INTEGER", nullable: false),
                    position = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owner_team", x => new { x.owner_id, x.team_number });
                    table.ForeignKey(
                        name: "FK_owner_team_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_owner_team_team_team_number",
                        column: x => x.team_number,
                        principalTable: "team",
                        principalColumn: "number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon_race",
                columns: table => new
                {
                    race_code = table.Column<string>(type: "TEXT", nullable: false),
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false),
                    mark = table.Column<int>(type: "INTEGER", nullable: false),
                    arrival_time = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon_race", x => new { x.race_code, x.country_code, x.year, x.ring_number });
                    table.ForeignKey(
                        name: "FK_pigeon_race_pigeon_country_code_year_ring_number",
                        columns: x => new { x.country_code, x.year, x.ring_number },
                        principalTable: "pigeon",
                        principalColumns: new[] { "country_code", "year", "ring_number" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_race_race_race_code",
                        column: x => x.race_code,
                        principalTable: "race",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon_sale",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    seller_id = table.Column<int>(type: "INTEGER", nullable: false),
                    buyer_id = table.Column<int>(type: "INTEGER", nullable: false),
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon_sale", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pigeon_sale_owner_buyer_id",
                        column: x => x.buyer_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_sale_owner_seller_id",
                        column: x => x.seller_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_sale_pigeon_country_code_year_ring_number",
                        columns: x => new { x.country_code, x.year, x.ring_number },
                        principalTable: "pigeon",
                        principalColumns: new[] { "country_code", "year", "ring_number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pigeon_swap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false),
                    player_id = table.Column<int>(type: "INTEGER", nullable: false),
                    coupled_player_id = table.Column<int>(type: "INTEGER", nullable: false),
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pigeon_swap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_coupled_player_id",
                        column: x => x.coupled_player_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_owner_player_id",
                        column: x => x.player_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pigeon_swap_pigeon_country_code_year_ring_number",
                        columns: x => new { x.country_code, x.year, x.ring_number },
                        principalTable: "pigeon",
                        principalColumns: new[] { "country_code", "year", "ring_number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selected_year_pigeon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false),
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selected_year_pigeon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_selected_year_pigeon_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_selected_year_pigeon_pigeon_country_code_year_ring_number",
                        columns: x => new { x.country_code, x.year, x.ring_number },
                        principalTable: "pigeon",
                        principalColumns: new[] { "country_code", "year", "ring_number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "selected_young_pigeon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    owner_id = table.Column<int>(type: "INTEGER", nullable: false),
                    country_code = table.Column<string>(type: "TEXT", nullable: false),
                    year = table.Column<int>(type: "INTEGER", nullable: false),
                    ring_number = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selected_young_pigeon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_selected_young_pigeon_owner_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owner",
                        principalColumn: "owner_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_selected_young_pigeon_pigeon_country_code_year_ring_number",
                        columns: x => new { x.country_code, x.year, x.ring_number },
                        principalTable: "pigeon",
                        principalColumns: new[] { "country_code", "year", "ring_number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_league_owner_league_rank",
                table: "league_owner",
                column: "league_rank");

            migrationBuilder.CreateIndex(
                name: "IX_league_owner_owner_id",
                table: "league_owner",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_owner_club_id",
                table: "owner",
                column: "club_id");

            migrationBuilder.CreateIndex(
                name: "IX_owner_team_owner_id",
                table: "owner_team",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_owner_team_team_number",
                table: "owner_team",
                column: "team_number");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_owner_id",
                table: "pigeon",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_race_country_code_year_ring_number",
                table: "pigeon_race",
                columns: new[] { "country_code", "year", "ring_number" });

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_sale_buyer_id",
                table: "pigeon_sale",
                column: "buyer_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_sale_country_code_year_ring_number",
                table: "pigeon_sale",
                columns: new[] { "country_code", "year", "ring_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_sale_seller_id",
                table: "pigeon_sale",
                column: "seller_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_country_code_year_ring_number",
                table: "pigeon_swap",
                columns: new[] { "country_code", "year", "ring_number" });

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_coupled_player_id",
                table: "pigeon_swap",
                column: "coupled_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_owner_id",
                table: "pigeon_swap",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_pigeon_swap_player_id",
                table: "pigeon_swap",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_race_type",
                table: "race",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_selected_year_pigeon_country_code_year_ring_number",
                table: "selected_year_pigeon",
                columns: new[] { "country_code", "year", "ring_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_selected_year_pigeon_owner_id",
                table: "selected_year_pigeon",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_selected_young_pigeon_country_code_year_ring_number",
                table: "selected_young_pigeon",
                columns: new[] { "country_code", "year", "ring_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_selected_young_pigeon_owner_id",
                table: "selected_young_pigeon",
                column: "owner_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "league_owner");

            migrationBuilder.DropTable(
                name: "owner_team");

            migrationBuilder.DropTable(
                name: "pigeon_race");

            migrationBuilder.DropTable(
                name: "pigeon_sale");

            migrationBuilder.DropTable(
                name: "pigeon_swap");

            migrationBuilder.DropTable(
                name: "selected_year_pigeon");

            migrationBuilder.DropTable(
                name: "selected_young_pigeon");

            migrationBuilder.DropTable(
                name: "league");

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
