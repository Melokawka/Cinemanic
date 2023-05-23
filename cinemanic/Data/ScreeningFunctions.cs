using AutoMapper;
using cinemanic.Models;
using System.Globalization;

namespace cinemanic.Data
{
    public class ScreeningFunctions
    {
        public static IEnumerable<string> GetPaginatedDates(List<string> sortedDates, int page, int pageSize)
        {
            return sortedDates.Skip((page - 1) * pageSize).Take(pageSize);
        }

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

        public static void SortScreeningsByDate(List<MovieInfo> moviesInfo)
        {
            foreach (var movie in moviesInfo)
            {
                movie.Screenings = movie.Screenings.OrderBy(s => s.ScreeningDate).ToList();
            }
        }

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
