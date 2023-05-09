using cinemanic.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace cinemanic.Data
{
    public class MoviesService
    {
        private static HttpClient httpClient = new();
        private static JsonSerializerOptions jsonOptions = new();

        private static int movieId;
        private static string apiEndpoint2;
        private static string apiEndpoint3;

        private static Random random = new Random();

        public static string FindTrailerKey(string json)
        {
            var jObject = JObject.Parse(json);
            var jArray = (JArray)jObject["results"];

            foreach (var item in jArray)
            {
                var type = (string)item["type"];
                if (type == "Trailer")
                {
                    return (string)item["key"];
                }
            }

            return null;
        }

        private static async Task<int> GetRandomId()
        {
            HttpResponseMessage existsHttp;

            do
            {
                movieId = random.Next(10000, 100000);
                Console.WriteLine(movieId);

                apiEndpoint2 = $"https://api.themoviedb.org/3/movie/{movieId}?api_key=4446cb535a867cc6db4c689c8ebc7d97";
                apiEndpoint3 = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key=4446cb535a867cc6db4c689c8ebc7d97";

                existsHttp = await httpClient.GetAsync(apiEndpoint2);

            } while (!existsHttp.IsSuccessStatusCode);

            return movieId;
        }

        //the errors are handled in getrandomid function
        private static async Task<Movie> GetMovie(CinemanicDbContext dbContext)
        {
            var response = await httpClient.GetFromJsonAsync<Movie>(apiEndpoint2, jsonOptions);

            var json = await httpClient.GetStringAsync(apiEndpoint3);

            var trailerLink = FindTrailerKey(json);

            //response.Trailer = !String.IsNullOrEmpty(trailerLink) ? "https://www.youtube.com/watch?v=" + trailerLink : "";
            response.Trailer = !String.IsNullOrEmpty(trailerLink) ? trailerLink : "";

            response.PosterPath = !String.IsNullOrEmpty(response.PosterPath) ? "https://image.tmdb.org/t/p/w200" + response.PosterPath : "";

            return response;
        }

        public static async Task GetMovies(CinemanicDbContext dbContext)
        {
            List<Genre> genres = new();
            List<Movie> movies = new();

            jsonOptions.Converters.Add(new MovieConverter());

            //retrieve 10 random movies from tmdb api
            for (int i = 0; i < 1; i++)
            {
                await GetRandomId();
                movies.Add(await GetMovie(dbContext));
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                // Load existing genres from the database based on the IDs in the movies
                var existingGenres = dbContext.Genres.ToList();

                foreach (Movie movie in movies)
                {
                    // Associate existing genres with the movie
                    movie.Genres = movie.Genres.Select(genre =>
                    {
                        var existingGenre = existingGenres.FirstOrDefault(eg => eg.Id == genre.Id);
                        return existingGenre ?? genre;
                    }).ToList();

                    dbContext.Movies.Add(movie);
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
