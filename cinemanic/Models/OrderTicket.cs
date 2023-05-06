namespace cinemanic.Models
{
    public class OrderTicket
    {
        public int OrderId { get; set; }
        public int TicketId { get; set; }
        public Order Order { get; set; }
        public Ticket Ticket { get; set; }
    }
}
