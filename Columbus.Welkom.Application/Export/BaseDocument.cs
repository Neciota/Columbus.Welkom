using Columbus.Welkom.Application.Models.DocumentModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;
public abstract class BaseDocument : IDocument
{
    private readonly BaseDocumentModel _documentModel;

    protected BaseDocument(BaseDocumentModel documentModel)
    {
        _documentModel = documentModel;
    }

    protected abstract string Title { get; }

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

    protected virtual void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Row(subrow =>
                {
                    subrow.RelativeItem().AlignLeft().Text(Title).FontSize(20);
                    subrow.RelativeItem().AlignRight().Text(_documentModel.ClubId.ToString()).FontSize(20);
                    subrow.RelativeItem().AlignRight().Text(_documentModel.Year.ToString()).FontSize(20);
                });
                column.Item().PaddingVertical(20).LineHorizontal(2);
            });
        });
    }

    protected abstract void ComposeContent(IContainer container);

    protected virtual void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().PaddingVertical(20).LineHorizontal(2);
                column.Item().Row(subrow =>
                {
                    subrow.RelativeItem().AlignLeft().Text(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    subrow.RelativeItem().AlignRight().Text(_documentModel.ClubId.ToString());
                    subrow.RelativeItem().AlignRight().Text(_documentModel.Year.ToString());
                });
                column.Item().Text(string.Join(",  ", _documentModel.RaceCodes)).FontSize(8);
            });
        });
    }
}
