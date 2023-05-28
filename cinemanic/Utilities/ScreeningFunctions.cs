using AutoMapper;
using cinemanic.Models;
using System.Globalization;

namespace cinemanic.Utilities
{
    /// <summary>
    /// Provides utility functions for screening operations.
    /// </summary>
    public class ScreeningFunctions
    {
        /// <summary>
        /// Retrieves a paginated subset of dates from a sorted list of dates.
        /// </summary>
        /// <param name="sortedDates">The sorted list of dates.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of dates per page.</param>
        /// <returns>An IEnumerable containing the paginated subset of dates.</returns>
        public static IEnumerable<string> GetPaginatedDates(List<string> sortedDates, int page, int pageSize)
        {
            return sortedDates.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Retrieves a sorted list of unique dates from a list of movie information.
        /// </summary>
        /// <param name="moviesInfo">The list of movie information.</param>
        /// /// <returns>The sorted list of unique dates.</returns>
        public static List<string> GetSortedUniqueDates(List<MovieInfo> moviesInfo)
        {
            var uniqueDates = new List<string>();

            foreach (var movie in moviesInfo)
            {
                foreach (var screening in movie.Screenings)
                {
                    if (!uniqueDates.Contains(screening.ScreeningDate.Date.ToString("dd-MM-yyyy")))
                    {
                        uniqueDates.Add(screening.ScreeningDate.Date.ToString("dd-MM-yyyy"));
                    }
                }
            }

            return uniqueDates.OrderBy(date => DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList();
        }

        /// <summary>
        /// Sorts the screenings within each movie information by date.
        /// </summary>
        /// <param name="moviesInfo">The list of movie information.</param>
        public static void SortScreeningsByDate(List<MovieInfo> moviesInfo)
        {
            foreach (var movie in moviesInfo)
            {
                movie.Screenings = movie.Screenings.OrderBy(s => s.ScreeningDate).ToList();
            }
        }

        /// <summary>
        /// Maps a list of movies to a list of movie information.
        /// </summary>
        /// <param name="movies">The list of movies.</param>
        /// <returns>The mapped list of movie information.</returns>
        public static List<MovieInfo> MapMoviesToMovieInfo(List<Movie> movies)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movie, MovieInfo>()
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.GenreName).ToList()))
                    .ForMember(dest => dest.Screenings, opt => opt.MapFrom(src => src.Screenings.Select(s => s).ToList()));

                cfg.CreateMap<Genre, string>().ConvertUsing(g => g.GenreName);
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<Movie>, List<MovieInfo>>(movies);
        }
    }
}
