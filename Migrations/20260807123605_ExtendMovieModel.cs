using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservationSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ExtendMovieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeRating",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cast",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImdbRating",
                table: "Movies",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Movies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Cast",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ImdbRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "Movies");
        }
    }
}
