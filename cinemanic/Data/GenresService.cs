using cinemanic.Data.Converters;
using cinemanic.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace cinemanic.Data
{
    public class GenresService
    {
        /// <summary>
        /// Retrieves the list of genres from an external API and saves them to the database.
        /// </summary>
        /// <param name="dbContext">The CinemanicDbContext instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task GetGenres(CinemanicDbContext dbContext)
        {
            string apiEndpoint = $"https://api.themoviedb.org/3/genre/movie/list?api_key=4446cb535a867cc6db4c689c8ebc7d97";
            var httpClient = new HttpClient();

            List<Genre> genreList = new();

            try
            {
                var jsonOptions = new JsonSerializerOptions();
                jsonOptions.Converters.Add(new GenreConverter());

                var response = await httpClient.GetStringAsync(apiEndpoint);

                if (response == null)
                {
                    throw new Exception("API response was null or invalid.");
                }

                genreList = JsonSerializer.Deserialize<List<Genre>>(response, jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Genres ON");

                await dbContext.Genres.AddRangeAsync(genreList);

                await dbContext.SaveChangesAsync();

                await dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Genres OFF");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
