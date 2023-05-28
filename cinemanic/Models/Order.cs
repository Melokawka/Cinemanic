namespace cinemanic.Models
{
    /// <summary>
    /// Represents an order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the ID of the order.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the total price of the order.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the ID of the associated account.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the status of the order.
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets the associated account.
        /// </summary>
        public Account Account { get; set; } = null!;

        /// <summary>
        /// Gets the list of tickets associated with the order.
        /// </summary>
        public List<Ticket> Tickets { get; } = new List<Ticket>();
    }

    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    public enum OrderStatus
    {
        PENDING,
        SUBMITTED,
        PAID,
        COMPLETED
    }
}
