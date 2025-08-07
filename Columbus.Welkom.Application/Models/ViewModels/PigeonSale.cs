using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class PigeonSale
{
    public int Id { get; set; }
    public Owner? Seller { get; set; }
    public Owner? Buyer { get; set; }
    public Pigeon? Pigeon { get; set; }
    public ICollection<RacePoints> RacePoints { get; set; } = [];
    public double TotalPoints => RacePoints.Sum(rp => rp.Points);
}
