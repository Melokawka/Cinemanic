using cinemanic.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace cinemanic.Data
{
    public class MovieService
    {
        private readonly CinemanicDbContext _dbContext;
        private readonly string _tmdbApiKey;

        private static HttpClient httpClient = new();
        private static JsonSerializerOptions jsonOptions = new();

        public MovieService(CinemanicDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _tmdbApiKey = configuration["TmdbApiKey"];
        }

        public async Task GetMovies()
        {
            List<Movie> movies = new();

            jsonOptions.Converters.Add(new MovieConverter());

            //retrieve random movies from tmdb api
            for (int i = 0; i < 5; i++)
            {
                movies.Add(await GetMovie());
            }

            var existingGenres = _dbContext.Genres.ToList();

            foreach (Movie movie in movies)
            {
                movie.Genres = movie.Genres.Select(genre =>
                {
                    var existingGenre = existingGenres.FirstOrDefault(eg => eg.Id == genre.Id);
                    return existingGenre ?? genre;
                }).ToList();

                _dbContext.Movies.Add(movie);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<Movie> GetMovie()
        {
            int movieId = await FindRandomMovieId();

            string apiEndpoint2 = $"https://api.themoviedb.org/3/movie/{movieId}?api_key=" + _tmdbApiKey;
            string apiEndpoint3 = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key=" + _tmdbApiKey;

            var response = await httpClient.GetFromJsonAsync<Movie>(apiEndpoint2, jsonOptions);

            var json = await httpClient.GetStringAsync(apiEndpoint3);

            var trailerLink = MovieServiceFunctions.FindTrailerKey(json);

            response.Trailer = !String.IsNullOrEmpty(trailerLink) ? trailerLink : "";

            response.PosterPath = !String.IsNullOrEmpty(response.PosterPath) ? response.PosterPath : "";

            return response;
        }

        private async Task<int> FindRandomMovieId()
        {
            HttpResponseMessage existsHttp;
            Random random = new();
            int movieId;

            do
            {
                movieId = random.Next(10000, 100000);

                existsHttp = await httpClient.GetAsync($"https://api.themoviedb.org/3/movie/{movieId}?api_key=" + _tmdbApiKey);

            } while (!existsHttp.IsSuccessStatusCode);

            return movieId;
        }

        public List<MovieInfo> GetMoviesInfo()
        {
            var movies = _dbContext.Movies
                .Include(m => m.Genres)
                .Include(m => m.Screenings)
                .ToList();

            return MovieServiceFunctions.PrepareMoviesInfo(movies);
        }
    }
}
