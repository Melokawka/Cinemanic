namespace cinemanic.Models
{
    /// <summary>
    /// Represents the view model for screening information.
    /// </summary>
    public class ScreeningViewModel
    {
        /// <summary>
        /// Gets or sets the list of current paginated dates.
        /// </summary>
        public List<string> CurrentPaginatedDates { get; set; }

        /// <summary>
        /// Gets or sets the list of movie information.
        /// </summary>
        public List<MovieInfo> MoviesInfo { get; set; } = new List<MovieInfo>();
    }
}
