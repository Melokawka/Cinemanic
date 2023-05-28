namespace cinemanic.Models
{
    /// <summary>
    /// Represents a view model for account information.
    /// </summary>
    public class AccountViewModel
    {
        /// <summary>
        /// Gets or sets the email of the account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the account.
        /// </summary>
        public DateTimeOffset BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the list of tickets associated with the account.
        /// </summary>
        public List<Ticket> Tickets { get; set; }

        /// <summary>
        /// Gets or sets the list of archived tickets associated with the account.
        /// </summary>
        public List<ArchivedTicket> ArchivedTickets { get; set; }

        /// <summary>
        /// Gets or sets the list of orders associated with the account.
        /// </summary>
        public List<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the list of likes associated with the account.
        /// </summary>
        public List<Like> Likes { get; set; }
    }
}
