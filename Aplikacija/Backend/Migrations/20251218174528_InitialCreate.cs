using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Oglasi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PostavljacOglasaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oglasi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Oglasi_Korisnici_PostavljacOglasaId",
                        column: x => x.PostavljacOglasaId,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oglasi_PostavljacOglasaId",
                table: "Oglasi",
                column: "PostavljacOglasaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oglasi");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
