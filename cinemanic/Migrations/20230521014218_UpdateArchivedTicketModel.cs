using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemanic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArchivedTicketModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ArchivedTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ArchivedTickets");
        }
    }
}
