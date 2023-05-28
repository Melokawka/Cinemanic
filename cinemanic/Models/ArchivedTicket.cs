namespace cinemanic.Models
{
    /// <summary>
    /// Represents an archived ticket.
    /// </summary>
    public class ArchivedTicket
    {
        /// <summary>
        /// Gets or sets the ID of the archived ticket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the seat number of the archived ticket.
        /// </summary>
        public int Seat { get; set; }

        /// <summary>
        /// Gets or sets the pricing type of the archived ticket.
        /// </summary>
        public PricingType PricingType { get; set; }

        /// <summary>
        /// Gets or sets the ticket price of the archived ticket.
        /// </summary>
        public decimal TicketPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the archived ticket is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the ID of the screening associated with the archived ticket.
        /// </summary>
        public int ScreeningId { get; set; }

        /// <summary>
        /// Gets or sets the screening associated with the archived ticket.
        /// </summary>
        public Screening Screening { get; set; }

        /// <summary>
        /// Gets or sets the screening date of the archived ticket.
        /// </summary>
        public DateTime ScreeningDate { get; set; }

        /// <summary>
        /// Gets or sets the archive date of the archived ticket.
        /// </summary>
        public DateTime ArchiveDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the order associated with the archived ticket.
        /// </summary>
        public int OrderId { get; set; }
    }
}
