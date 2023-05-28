using cinemanic.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cinemanic.Data.Converters
{
    public class GenreConverter : JsonConverter<List<Genre>>
    {
        /// <inheritdoc/>
        public override List<Genre> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var genres = new List<Genre>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    // removing breaks parser
                    reader.Read();

                    return genres;
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    continue;
                }

                var genre = new Genre();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        genres.Add(genre);
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        continue;
                    }

                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "id":
                            genre.Id = reader.GetInt32();
                            break;
                        case "name":
                            genre.GenreName = reader.GetString();
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
            }

            return genres;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, List<Genre> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
