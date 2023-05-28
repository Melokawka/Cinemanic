using cinemanic.Data.Converters;
using cinemanic.Models;
using cinemanic.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace cinemanic.Data
{
    /// <summary>
    /// Service for interacting with movie data.
    /// </summary>
    public class MovieService
    {
        private readonly CinemanicDbContext _dbContext;
        private readonly string _tmdbApiKey;

        private static HttpClient httpClient = new();
        private static JsonSerializerOptions jsonOptions = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieService"/> class.
        /// </summary>
        /// <param name="dbContext">The CinemanicDbContext instance.</param>
        /// <param name="configuration">The IConfiguration instance.</param>
        public MovieService(CinemanicDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _tmdbApiKey = configuration["TmdbApiKey"];
        }

        /// <summary>
        /// Retrieves movies from the TMDB API and saves them to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves a movie from the TMDB API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Finds a random movie ID from the TMDB API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Retrieves movie information including genres and screenings.
        /// </summary>
        /// <returns>A list of MovieInfo objects.</returns>
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
