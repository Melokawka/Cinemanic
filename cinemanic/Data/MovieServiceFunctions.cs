using AutoMapper;
using cinemanic.Models;
using Newtonsoft.Json.Linq;

namespace cinemanic.Data
{
    public class MovieServiceFunctions
    {
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
