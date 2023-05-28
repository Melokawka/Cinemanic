using Microsoft.EntityFrameworkCore;
using Stripe;

namespace cinemanic.Data
{
    /// <summary>
    /// Service for managing Stripe products.
    /// </summary>
    public class StripeProductService
    {
        private readonly CinemanicDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripeProductService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public StripeProductService(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds Stripe products for the movies in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddStripeProducts()
        {
            List<Models.Movie> movies = await _dbContext.Movies.ToListAsync();

            foreach (var movie in movies)
            {
                var options = new ProductCreateOptions
                {
                    Id = movie.Id.ToString(),
                    Name = movie.Title,
                    Images = new List<string> { "https://image.tmdb.org/t/p/w500/" + movie.PosterPath },
                };

                var service = new ProductService();
                service.Create(options);
            }
        }

        /// <summary>
        /// Checks if there are any products in the Stripe account.
        /// </summary>
        /// <returns><c>true</c> if there are products; otherwise, <c>false</c>.</returns>
        public async Task<bool> HasProducts()
        {
            var productService = new ProductService();
            var products = await productService.ListAsync();

            return products.Any();
        }
    }
}
