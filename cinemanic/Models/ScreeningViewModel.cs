namespace cinemanic.Models
{
    public class ScreeningViewModel
    {
        public string CurrentPaginatedDate { get; set; }
        public List<MovieInfo> MoviesInfo { get; set; } = new();
    }
}
