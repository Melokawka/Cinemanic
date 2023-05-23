using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemanic.Migrations
{
    /// <inheritdoc />
    public partial class AddArchivedScreenings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "ArchivedScreenings",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false),
                ScreeningDate = table.Column<DateTime>(nullable: false),
                Subtitles = table.Column<bool>(nullable: false),
                Lector = table.Column<bool>(nullable: false),
                Dubbing = table.Column<bool>(nullable: false),
                Is3D = table.Column<bool>(nullable: false),
                SeatsLeft = table.Column<int>(nullable: false),
                RoomId = table.Column<int>(nullable: false),
                MovieId = table.Column<int>(nullable: false),
                GrossIncome = table.Column<decimal>(nullable: false, precision: 7, scale: 2)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ArchivedScreenings", x => x.Id);
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedScreenings");
        }
    }
}
