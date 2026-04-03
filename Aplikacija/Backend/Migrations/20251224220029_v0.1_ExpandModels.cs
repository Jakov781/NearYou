using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class v01_ExpandModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adresa",
                table: "Oglasi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Cena",
                table: "Oglasi",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Grad",
                table: "Oglasi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Oglasi",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Oglasi",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Biografija",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlikaURL",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "Korisnici",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VestineJson",
                table: "Korisnici",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresa",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "Cena",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "Grad",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Oglasi");

            migrationBuilder.DropColumn(
                name: "Biografija",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "SlikaURL",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "Korisnici");

            migrationBuilder.DropColumn(
                name: "VestineJson",
                table: "Korisnici");
        }
    }
}
