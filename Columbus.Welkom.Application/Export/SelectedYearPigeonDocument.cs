using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;

public class SelectedYearPigeonDocument : IDocument
{
    private readonly SelectedYearPigeon _selectedYearPigeon;

    public SelectedYearPigeonDocument(SelectedYearPigeon selectedYearPigeon)
    {
        _selectedYearPigeon = selectedYearPigeon;
    }

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Row(subrow =>
                {
                    subrow.RelativeItem().AlignLeft().Text("Tientjesduif").FontSize(20);
                    subrow.RelativeItem().AlignRight().Text(_selectedYearPigeon.ClubId.ToString()).FontSize(20);
                    subrow.RelativeItem().AlignRight().Text(_selectedYearPigeon.Year.ToString()).FontSize(20);
                });
                column.Item().PaddingVertical(20).LineHorizontal(2);
            });
        });
    }

    private void ComposeContent(IContainer container)
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

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().PaddingVertical(20).LineHorizontal(2);
                column.Item().Row(subrow =>
                {
                    subrow.RelativeItem().AlignLeft().Text(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    subrow.RelativeItem().AlignRight().Text(_selectedYearPigeon.ClubId.ToString());
                    subrow.RelativeItem().AlignRight().Text(_selectedYearPigeon.Year.ToString());
                });
                column.Item().Text(string.Join(",  ", _selectedYearPigeon.RaceCodes)).FontSize(8);
            });
        });
    }
}
