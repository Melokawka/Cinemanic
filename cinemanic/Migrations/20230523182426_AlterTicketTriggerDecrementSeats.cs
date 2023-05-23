using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemanic.Migrations
{
    /// <inheritdoc />
    public partial class AlterTicketTriggerDecrementSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TRIGGER UpdateOrderTotalPrice
                ON Tickets
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Update TotalPrice in Orders table
                    UPDATE Orders
                    SET TotalPrice = 
                    (
                        SELECT SUM(TicketPrice)
                        FROM Tickets
                        WHERE OrderId = inserted.OrderId
                    )
                    FROM Orders
                    INNER JOIN inserted ON Orders.Id = inserted.OrderId;

                    -- Decrement SeatsLeft in Screenings table
                    UPDATE Screenings
                    SET SeatsLeft = SeatsLeft - 1
                    WHERE Id IN (SELECT ScreeningId FROM inserted WHERE IsActive = 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
