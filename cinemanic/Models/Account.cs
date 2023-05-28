namespace cinemanic.Models
{
    /// <summary>
    /// Represents a user account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user email associated with the account.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the birthdate of the account user.
        /// </summary>
        public DateTime Birthdate { get; set; }

        /// <summary>
        /// Gets or sets the newsletter client associated with the account.
        /// </summary>
        public NewsletterClient NewsletterClient { get; set; } = null!;

        /// <summary>
        /// Gets the list of likes associated with the account.
        /// </summary>
        public List<Like> Likes { get; } = new();

        /// <summary>
        /// Gets the list of orders associated with the account.
        /// </summary>
        public List<Order> Orders { get; } = new();
    }
}
