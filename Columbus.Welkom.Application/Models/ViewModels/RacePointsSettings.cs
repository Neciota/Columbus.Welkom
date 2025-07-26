using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class RacePointsSettings
    {
        public RaceType RaceType { get; set; }
        public int PointsQuotient { get; set; }
        public int MinPoints { get; set; } 
        public int MaxPoints { get; set; }
        public int DecimalCount { get; set; }
    }
}
