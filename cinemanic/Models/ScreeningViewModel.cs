namespace cinemanic.Models
{
    public class ScreeningViewModel
    {
        public List<string> CurrentPaginatedDates { get; set; }
        public List<MovieInfo> MoviesInfo { get; set; } = new();
    }
}
