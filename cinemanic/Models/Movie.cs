using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Bogus.DataSets.Name;

namespace cinemanic.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Duration { get; set; }
        public bool PolishMade { get; set; }
        public string Description { get; set; }
        public int? Budget { get; set; }
        public string Trailer { get; set; }
        public string PosterPath { get; set; }
        public int? TmdbPopularity { get; set; }
        public bool Adult { get; set; }

        public List<Screening> Screenings { get; } = new();
        public List<Like> Likes { get; } = new();
        public List<Genre> Genres { get; set;  } = new();
    }
}
