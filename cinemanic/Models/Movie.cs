using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinemanic.Models
{
    public class Movie
    {
        [Key]
        [Column("movie_id")]
        public int MovieId { get; set; }
        public string Title { get; set; }
        [Column("release_date")]
        public DateTime? ReleaseDate { get; set; }
        public int? Duration { get; set; }
        [Column("polish_made")]
        public bool PolishMade { get; set; }
        public string Description { get; set; }
        public int? Budget { get; set; }
        public string Trailer { get; set; }
        [Column("poster_path")]
        public string PosterPath { get; set; }
        [Column("tmdb_popularity")]
        public int? TmdbPopularity { get; set; }
        public bool Adult { get; set; }
    }
}
