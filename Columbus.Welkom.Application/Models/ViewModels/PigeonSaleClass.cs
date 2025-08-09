namespace Columbus.Welkom.Application.Models.ViewModels;
public class PigeonSaleClass
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<PigeonSale> PigeonSales { get; set; } = [];
}
