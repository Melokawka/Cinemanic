namespace cinemanic.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int Seat { get; set; }
        public PricingType PricingType { get; set; }
        public decimal? TicketPrice { get; set; }
        public int ScreeningId { get; set; }
        public Screening Screening { get; set; } = null!;
        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }
    }

    public enum PricingType
    {
        ULGOWY,
        NORMALNY
    }
}
