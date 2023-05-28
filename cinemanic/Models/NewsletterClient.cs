using System.ComponentModel.DataAnnotations.Schema;

namespace cinemanic.Models
{
    /// <summary>
    /// Represents a newsletter client.
    /// </summary>
    [Table("NewsletterClients")]
    public class NewsletterClient
    {
        /// <summary>
        /// Gets or sets the ID of the associated account.
        /// </summary>
        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the associated account.
        /// </summary>
        public Account Account { get; set; } = null!;
    }
}
