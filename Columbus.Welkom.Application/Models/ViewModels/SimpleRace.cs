using Columbus.Models;
using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.ViewModels
{
    public class SimpleRace
    {
        public SimpleRace(int? number, RaceType type, string name, string code, DateTime startTime, Coordinate location, int ownerCount, int pigeonCount)
        {
            Number = number;
            Type = type;
            Name = name;
            Code = code;
            StartTime = startTime;
            Location = location;
            OwnerCount = ownerCount;
            PigeonCount = pigeonCount;
        }

        public int? Number { get; set; }
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime StartTime { get; set; }
        public Coordinate Location { get; set; }
        public int OwnerCount { get; set; }
        public int PigeonCount { get; set; }
    }
}
