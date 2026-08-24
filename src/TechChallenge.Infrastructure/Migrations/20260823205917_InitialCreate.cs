using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TechChallenge.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Games", columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                IsMultiplayer = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
            });

            migrationBuilder.CreateTable(name: "Promotions", columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Promotions", x => x.Id);
            });

            migrationBuilder.CreateTable(name: "Users", columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                Password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Perfil = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "DATETIME", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

            migrationBuilder.CreateTable(name: "GamePromotions", columns: table => new
            {
                GamesId = table.Column<int>(type: "int", nullable: false),
                PromotionsId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GamePromotions", x => new { x.GamesId, x.PromotionsId });
                table.ForeignKey(
                    name: "FK_GamePromotions_Games_GamesId",
                    column: x => x.GamesId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GamePromotions_Promotions_PromotionsId",
                    column: x => x.PromotionsId,
                    principalTable: "Promotions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateTable(name: "UserGames", columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                GameId = table.Column<int>(type: "int", nullable: false),
                PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserGames", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserGames_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserGames_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateIndex(
                name: "IX_GamePromotions_PromotionsId",
                table: "GamePromotions",
                column: "PromotionsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_GameId",
                table: "UserGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId_GameId",
                table: "UserGames",
                columns: new[] { "UserId", "GameId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "GamePromotions");

            migrationBuilder.DropTable(name: "UserGames");

            migrationBuilder.DropTable(name: "Promotions");

            migrationBuilder.DropTable(name: "Games");

            migrationBuilder.DropTable(name: "Users");
        }
    }
}