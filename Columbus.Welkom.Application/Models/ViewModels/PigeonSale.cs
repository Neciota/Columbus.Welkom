using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.ViewModels;

public class PigeonSale
{
    public int Id { get; set; }
    public Owner? Seller { get; set; }
    public Owner? Buyer { get; set; }
    public Pigeon? Pigeon { get; set; }
    public double Points { get; set; }
}
