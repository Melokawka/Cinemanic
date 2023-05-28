namespace cinemanic.Models
{
    /// <summary>
    /// Represents a ticket for a screening.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Gets or sets the ticket ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the seat number.
        /// </summary>
        public int Seat { get; set; }

        /// <summary>
        /// Gets or sets the pricing type of the ticket.
        /// </summary>
        public PricingType PricingType { get; set; }

        /// <summary>
        /// Gets or sets the price of the ticket.
        /// </summary>
        public decimal TicketPrice { get; set; }

        /// <summary>
        /// Gets or sets the ID of the associated screening.
        /// </summary>
        public int ScreeningId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ticket is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the associated screening.
        /// </summary>
        public Screening Screening { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated order.
        /// </summary>
        public Order Order { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the associated order.
        /// </summary>
        public int OrderId { get; set; }
    }

    /// <summary>
    /// Represents the pricing type of a ticket.
    /// </summary>
    public enum PricingType
    {
        /// <summary>
        /// Normal pricing type.
        /// </summary>
        NORMALNY,

        /// <summary>
        /// Discounted pricing type.
        /// </summary>
        ULGOWY,
    }
}
