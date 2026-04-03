using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class AddChatAndMessagess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chatovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OglasId = table.Column<int>(type: "int", nullable: false),
                    KlijentId = table.Column<int>(type: "int", nullable: false),
                    OglasivacId = table.Column<int>(type: "int", nullable: false),
                    Kreiran = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoslednjaPorukaVreme = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PoslednjaPoruka = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoslednjaPorukaPosiljalac = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chatovi_Korisnici_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chatovi_Korisnici_OglasivacId",
                        column: x => x.OglasivacId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chatovi_Oglasi_OglasId",
                        column: x => x.OglasId,
                        principalTable: "Oglasi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Poruke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    PosiljalacId = table.Column<int>(type: "int", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    VremeSlanja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poruke_Chatovi_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chatovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_PosiljalacId",
                        column: x => x.PosiljalacId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatovi_KlijentId",
                table: "Chatovi",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_Chatovi_OglasId_KlijentId",
                table: "Chatovi",
                columns: new[] { "OglasId", "KlijentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chatovi_OglasivacId",
                table: "Chatovi",
                column: "OglasivacId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_ChatId",
                table: "Poruke",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_PosiljalacId",
                table: "Poruke",
                column: "PosiljalacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Poruke");

            migrationBuilder.DropTable(
                name: "Chatovi");
        }
    }
}
