using Microsoft.EntityFrameworkCore;
using Stripe;

namespace cinemanic.Data
{
    public class StripeProductService
    {
        private readonly CinemanicDbContext _dbContext;
        public StripeProductService(CinemanicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        public async Task<bool> HasProducts()
        {
            var productService = new ProductService();
            var products = await productService.ListAsync();

            return products.Any();
        }
    }
}
