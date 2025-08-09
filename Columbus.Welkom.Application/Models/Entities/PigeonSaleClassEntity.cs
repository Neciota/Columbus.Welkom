namespace Columbus.Welkom.Application.Models.Entities;

public class PigeonSaleClassEntity : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<PigeonSaleEntity> PigeonSales { get; set; } = [];
}
