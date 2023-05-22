using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemanic.Migrations
{
    /// <inheritdoc />
    public partial class AlterTicketTrigger : Migration
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

                    UPDATE Orders
                    SET TotalPrice = 
                    (
                        SELECT SUM(TicketPrice)
                        FROM Tickets
                        WHERE OrderId = inserted.OrderId
                    )
                    FROM Orders
                    INNER JOIN inserted ON Orders.Id = inserted.OrderId;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
