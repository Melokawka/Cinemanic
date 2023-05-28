using AutoMapper;
using cinemanic.Models;
using Newtonsoft.Json.Linq;

namespace cinemanic.Utilities
{
    /// <summary>
    /// Helper functions for movie-related operations.
    /// </summary>
    public class MovieServiceFunctions
    {
        /// <summary>
        /// Prepares movie information by mapping it to MovieInfo objects and ordering screenings.
        /// </summary>
        /// <param name="movies">The list of Movie objects.</param>
        /// <returns>A list of MovieInfo objects.</returns>
        public static List<MovieInfo> PrepareMoviesInfo(List<Movie> movies)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieInfo>()
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName).ToList()))
                    .ForMember(dest => dest.Screenings, opt => opt.MapFrom(src => src.Screenings.Select(s => s).ToList()));

                cfg.CreateMap<Genre, string>().ConvertUsing(g => g.GenreName);
            });

            IMapper mapper = config.CreateMapper();
            var moviesInfo = mapper.Map<List<Movie>, List<MovieInfo>>(movies);

            foreach (var movie in moviesInfo)
            {
                movie.Screenings = movie.Screenings.OrderBy(s => s.ScreeningDate).ToList();
            }

            return moviesInfo;
        }

        /// <summary>
        /// Finds the trailer key from the JSON response.
        /// </summary>
        /// <param name="json">The JSON response containing movie data.</param>
        /// <returns>The trailer key if found, or null.</returns>
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
    }
}
