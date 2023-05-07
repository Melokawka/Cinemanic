using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace cinemanic.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string GenreName { get; set; }
        public List<Movie> Movies { get; set; } = new();
    }
}
