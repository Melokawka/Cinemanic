using cinemanic.Models;
using Stripe;
using Stripe.Checkout;

namespace cinemanic.Data
{
    public class ShoppingCartFunctions
    {
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

        public static int GetUserAge(DateTime userBirthDate)
        {
            DateTime currentDate = DateTime.Now;

            int userAge = currentDate.Year - userBirthDate.Year;

            bool hasHadBirthdayThisYear = (currentDate > userBirthDate.AddYears(userAge));

            if (!hasHadBirthdayThisYear)
            {
                userAge--;
            }

            return userAge;
        }

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
