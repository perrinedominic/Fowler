using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLibrary.Migrations
{
    public partial class AddRatingAndPlatform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Platforms",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Games",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platforms",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Games");
        }
    }
}
