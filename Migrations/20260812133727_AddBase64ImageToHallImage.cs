using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservationSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBase64ImageToHallImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Scaffolded as a rename (not drop+add) so existing hall image URLs survive —
            // EF's default diff can't tell a rename from a drop+add and would otherwise
            // silently delete every existing HallImages.ImageUrl value.
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "HallImages",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "Base64Image",
                table: "HallImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base64Image",
                table: "HallImages");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "HallImages",
                newName: "ImageUrl");
        }
    }
}
