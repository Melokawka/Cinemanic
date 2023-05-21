namespace cinemanic.Models
{
    public class ArchivedTicket
    {
        public int Id { get; set; }
        public int Seat { get; set; }
        public PricingType PricingType { get; set; }
        public decimal TicketPrice { get; set; }
        public bool IsActive { get; set; }
        public int ScreeningId { get; set; }
        public Screening Screening { get; set; }
        public DateTime ScreeningDate { get; set; }
        public DateTime ArchiveDate { get; set; }
        public int OrderId { get; set; }
    }
}
