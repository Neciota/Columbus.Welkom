using Columbus.Models.Owner;
using Columbus.Models.Pigeon;

namespace Columbus.Welkom.Application.Models.Entities;

public class PigeonSaleEntity : IEntity
{
    private CountryCode _countryCode;
    private int _year;
    private RingNumber _ringNumber;

    public int Id { get; set; }
    public PigeonId PigeonId
    {
        get => PigeonId.Create(_countryCode, _year, _ringNumber);
        set
        {
            _countryCode = value.CountryCode;
            _year = value.Year;
            _ringNumber = value.RingNumber;
        }
    }
    public OwnerId SellerId { get; set; }
    public OwnerId BuyerId { get; set; }

    public OwnerEntity? Seller { get; set; }
    public OwnerEntity? Buyer { get; set; }
    public PigeonEntity? Pigeon { get; set; }
}
