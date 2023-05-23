﻿namespace cinemanic.Models
{
    public class ArchivedScreening
    {
        public int Id { get; set; }
        public DateTime ScreeningDate { get; set; }
        public bool Subtitles { get; set; }
        public bool Lector { get; set; }
        public bool Dubbing { get; set; }
        public bool Is3D { get; set; }
        public int SeatsLeft { get; set; }
        public int RoomId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int MovieId { get; set; }
        public decimal GrossIncome { get; set; }
    }
}