using Columbus.Welkom.Application.Models.ViewModels;
using System.Text;

namespace Columbus.Welkom.Application.Export
{
    public class PigeonSwapDocument
    {
        private readonly IEnumerable<PigeonSwapPair> _pigeonSwapPairs;

        public PigeonSwapDocument(IEnumerable<PigeonSwapPair> pigeonSwapPairs) 
        {
            _pigeonSwapPairs = pigeonSwapPairs;
        }

        public byte[] GetDocument()
        {
            byte[] comma = Encoding.ASCII.GetBytes(",");
            byte[] newLine = Encoding.ASCII.GetBytes("\n");
            List<byte> document = new List<byte>();

            document.AddRange(Encoding.ASCII.GetBytes("zetter"));
            document.AddRange(comma);
            document.AddRange(Encoding.ASCII.GetBytes("duif"));
            document.AddRange(comma);
            document.AddRange(Encoding.ASCII.GetBytes("gekoppelde speler"));
            foreach (var racePoints in _pigeonSwapPairs.First().RacePoints!)
            {
                document.AddRange(comma);
                document.AddRange(Encoding.ASCII.GetBytes(racePoints.Key.Name));
            }

            document.AddRange(newLine);

            foreach (PigeonSwapPair pigeonSwapPair in _pigeonSwapPairs)
            {
                document.AddRange(Encoding.ASCII.GetBytes(pigeonSwapPair.Player!.Name));
                document.AddRange(comma);
                document.AddRange(Encoding.ASCII.GetBytes(pigeonSwapPair.Pigeon!.ToString()));
                document.AddRange(comma);
                document.AddRange(Encoding.ASCII.GetBytes(pigeonSwapPair.CoupledPlayer!.Name));

                foreach (var racePoints in pigeonSwapPair.RacePoints!)
                {
                    document.AddRange(comma);
                    document.AddRange(Encoding.ASCII.GetBytes(racePoints.Value.ToString()));
                }

                document.AddRange(newLine);
            }

            return document.ToArray();
        }
    }
}
