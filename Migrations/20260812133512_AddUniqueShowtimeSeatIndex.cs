using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieReservationSystem.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueShowtimeSeatIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowtimeId",
                table: "BookedSeats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Backfill the new column from each row's parent booking before the unique index
            // goes on — existing rows would otherwise all sit at the AddColumn default (0) and
            // very likely collide with each other on (ShowtimeId, SeatId), which would make the
            // CreateIndex below fail on any database with real booking data in it.
            migrationBuilder.Sql(@"
                UPDATE bs
                SET bs.ShowtimeId = b.ShowtimeId
                FROM BookedSeats bs
                INNER JOIN Bookings b ON b.Id = bs.BookingId;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_BookedSeats_ShowtimeId_SeatId",
                table: "BookedSeats",
                columns: new[] { "ShowtimeId", "SeatId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookedSeats_ShowtimeId_SeatId",
                table: "BookedSeats");

            migrationBuilder.DropColumn(
                name: "ShowtimeId",
                table: "BookedSeats");
        }
    }
}
