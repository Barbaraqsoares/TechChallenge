using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(2000)", nullable: false),
                    Preco = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Genero = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    DataLancamento = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    PercentualDesconto = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    SenhaHash = table.Column<string>(type: "VARCHAR(200)", nullable: false),
                    Perfil = table.Column<int>(type: "INT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioJogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "INT", nullable: false),
                    JogoId = table.Column<int>(type: "INT", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    DataAquisicao = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioJogo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioJogo_Jogo_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioJogo_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jogo_Titulo",
                table: "Jogo",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogo_JogoId",
                table: "UsuarioJogo",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioJogo_UsuarioId_JogoId",
                table: "UsuarioJogo",
                columns: new[] { "UsuarioId", "JogoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioJogo");

            migrationBuilder.DropTable(
                name: "Jogo");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
