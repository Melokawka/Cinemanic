using cinemanic.Models;
using Stripe;
using Stripe.Checkout;

namespace cinemanic.Utilities
{
    /// <summary>
    /// Provides utility functions for shopping cart operations.
    /// </summary>
    public class ShoppingCartFunctions
    {
        /// <summary>
        /// Prepares line items for a given list of tickets.
        /// </summary>
        /// <param name="tickets">The list of tickets.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the prepared line items.</returns>
        public static async Task<List<SessionLineItemOptions>> PrepareLineItems(List<Ticket> tickets)
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var ticket in tickets)
            {
                var price = ticket.TicketPrice;

                var productService = new ProductService();
                var products = await productService.ListAsync();

                // Find the corresponding Stripe product
                var product = products.FirstOrDefault(p => p.Name == ticket.Screening.Movie.Title);

                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "PLN",
                        UnitAmount = (long)(price * 100), // Price in grosze
                        Product = product?.Id,
                    },
                    Quantity = 1,
                };

                lineItems.Add(lineItem);
            }

            return lineItems;
        }

        /// <summary>
        /// Gets the age of a user based on their birth date.
        /// </summary>
        /// <param name="userBirthDate">The user's birth date.</param>
        /// <returns>The age of the user.</returns>
        public static int GetUserAge(DateTime userBirthDate)
        {
            DateTime currentDate = DateTime.Now;

            int userAge = currentDate.Year - userBirthDate.Year;

            bool hasHadBirthdayThisYear = currentDate > userBirthDate.AddYears(userAge);

            if (!hasHadBirthdayThisYear)
            {
                userAge--;
            }

            return userAge;
        }

        /// <summary>
        /// Checks if a user can purchase adult content based on their age and the order.
        /// </summary>
        /// <param name="userAge">The age of the user.</param>
        /// <param name="order">The order.</param>
        /// <returns>True if the user can purchase adult content, otherwise false.</returns>
        public static bool CanPurchaseAdultContent(int userAge, Order order)
        {
            foreach (var ticket in order.Tickets)
            {
                if (userAge < 18 && ticket.Screening.Movie.Adult)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
