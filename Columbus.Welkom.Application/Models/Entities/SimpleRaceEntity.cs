using Columbus.Models;
using Columbus.Models.Race;

namespace Columbus.Welkom.Application.Models.Entities
{
    public class SimpleRaceEntity
    {
        public SimpleRaceEntity(int? number, RaceType type, string name, string code, DateTime startTime, double latitude, double longitude, int ownerCount, int pigeonCount)
        {
            Number = number;
            Type = type;
            Name = name;
            Code = code;
            StartTime = startTime;
            Latitude = latitude;
            Longitude = longitude;
            OwnerCount = ownerCount;
            PigeonCount = pigeonCount;
        }

        public int Id { get; set; }
        public int? Number { get; set; }
        public RaceType Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime StartTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int OwnerCount { get; set; }
        public int PigeonCount { get; set; }
        public SimpleRace ToSimpleRace()
        {
            Coordinate startLocation = new Coordinate(Longitude, Latitude);

            return new SimpleRace(Number, Type, Name, Code, StartTime, startLocation, OwnerCount, PigeonCount);
        }
    }
}
