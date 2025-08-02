using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;

public class SelectedYearPigeonDocument : BaseDocument
{
    private readonly SelectedYearPigeon _selectedYearPigeon;

    public SelectedYearPigeonDocument(SelectedYearPigeon selectedYearPigeon) : base(selectedYearPigeon)
    {
        _selectedYearPigeon = selectedYearPigeon;
    }

    protected override string Title => "Tientjesduif";

    protected override void ComposeContent(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.RelativeColumn(6);
                columns.RelativeColumn(6);
                columns.RelativeColumn(1);
            });

            int position = 0;
            foreach (OwnerPigeonPair ownerPigeonPair in _selectedYearPigeon.OwnerPigeonPairs)
            {
                position++;
                table.Cell().Text($"{position}.").LineHeight(1.5f);
                table.Cell().Text(ownerPigeonPair.Owner?.Name).LineHeight(1.5f);
                table.Cell().Text(ownerPigeonPair.Pigeon?.Id.ToString()).LineHeight(1.5f);
                table.Cell().Text(ownerPigeonPair.Points.ToString()).LineHeight(1.5f);
            }
        });
    }
}
