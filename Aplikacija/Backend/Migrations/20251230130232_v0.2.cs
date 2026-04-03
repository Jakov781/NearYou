using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class v02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumKreiranja",
                table: "Oglasi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "KategorijaId",
                table: "Oglasi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Oglasi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipCene",
                table: "Oglasi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Kategorije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorije", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oglasi_KategorijaId",
                table: "Oglasi",
                column: "KategorijaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Oglasi_Kategorije_KategorijaId",
                table: "Oglasi",
                column: "KategorijaId",
                principalTable: "Kategorije",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Oglasi_Kategorije_KategorijaId",
                table: "Oglasi");

            migrationBuilder.DropTable(
                name: "Kategorije");

            migrationBuilder.DropIndex(
                name: "IX_Oglasi_KategorijaId",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "DatumKreiranja",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "KategorijaId",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "SlikaUrl",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "TipCene",
                table: "Oglasi");
        }
    }
}
