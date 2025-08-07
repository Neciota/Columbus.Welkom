using Columbus.Models.Race;
using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;

public class PigeonSaleDocument(PigeonSales pigeonSales) : BaseDocument(pigeonSales)
{
    private readonly PigeonSales _pigeonSales = pigeonSales;

    protected override string Title => "Duivenverkoop";

    protected override PageSize PageSize => PageSizes.A4.Landscape();

    protected override void ComposeContent(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(0.5f);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
                foreach (SimpleRace simpleRace in _pigeonSales.Races)
                {
                    columns.RelativeColumn(1);
                }
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Text(string.Empty);
                header.Cell().Text("Schenker");
                header.Cell().Text("Koper");
                header.Cell().Text("Duif");
                foreach (SimpleRace simpleRace in _pigeonSales.Races)
                {
                    header.Cell().Text(simpleRace.Name).LineHeight(1.5f);
                }
                header.Cell().Text("Totaal");
            });

            int position = 0;
            foreach (PigeonSale pigeonSale in _pigeonSales.AllPigeonSales.OrderByDescending(to => to.TotalPoints))
            {
                position++;
                table.Cell().Text($"{position}.").LineHeight(1.5f);
                table.Cell().Text(pigeonSale.Seller?.Name).LineHeight(1.5f);
                table.Cell().Text(pigeonSale.Buyer?.Name).LineHeight(1.5f);
                table.Cell().Text(pigeonSale.Pigeon?.Id.ToString()).LineHeight(1.5f);
                foreach (SimpleRace simpleRace in _pigeonSales.Races)
                {
                    table.Cell().Text((pigeonSale.RacePoints.FirstOrDefault(rp => rp.RaceCode == simpleRace.Code)?.Points ?? 0d).ToString("N0")).LineHeight(1.5f);
                }
                table.Cell().Text(pigeonSale.TotalPoints.ToString("N0")).LineHeight(1.5f);
            }
        });
    }
}
