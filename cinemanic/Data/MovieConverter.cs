using cinemanic.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cinemanic.Data
{
    public class MovieConverter : JsonConverter<Movie>
    {
        public override Movie Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var movie = new Movie();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return movie;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "title":
                        movie.Title = reader.GetString();
                        break;
                    case "release_date":
                        movie.ReleaseDate = DateTime.Parse(reader.GetString());
                        break;
                    case "runtime":
                        movie.Duration = reader.GetInt32();
                        break;
                    case "original_language":
                        movie.PolishMade = reader.GetString() == "pl";
                        break;
                    case "overview":
                        movie.Description = reader.GetString();
                        break;
                    case "budget":
                        movie.Budget = reader.GetInt32();
                        break;
                    case "poster_path":
                        movie.PosterPath = reader.GetString();
                        break;
                    case "popularity":
                        movie.TmdbPopularity = (int)(reader.GetDouble() * 1000);
                        break;
                    case "adult":
                        movie.Adult = reader.GetBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return movie;
        }

        public override void Write(Utf8JsonWriter writer, Movie value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
