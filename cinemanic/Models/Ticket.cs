namespace cinemanic.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int Seat { get; set; }
        public string PricingType { get; set; }
        public decimal? TicketPrice { get; set; }
        public int ScreeningId { get; set; }
        public Screening Screening { get; set; }
    }
}
