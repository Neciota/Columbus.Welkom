using Columbus.Welkom.Application.Models.DocumentModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;

public class LeaguesDocument : BaseDocument
{
    private readonly Leagues _leagues;

    public LeaguesDocument(Leagues leagues) : base(leagues)
    {
        _leagues = leagues;
    }

    protected override string Title => "Divisiespel";

    protected override void ComposeContent(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                foreach (Models.ViewModels.League league in _leagues.AllLeagues)
                {
                    column.Item().PaddingVertical(8).PreventPageBreak().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(6);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().ColumnSpan(3).Text(league.Name);
                        });

                        int position = 0;
                        foreach (Models.ViewModels.LeagueOwner leagueOwner in league.LeagueOwners.OrderByDescending(lo => lo.Points))
                        {
                            position++;
                            table.Cell().Text($"{position}.").LineHeight(1.5f);
                            table.Cell().Text(leagueOwner.Owner?.Name).LineHeight(1.5f);
                            table.Cell().Text(leagueOwner.Points.ToString()).LineHeight(1.5f);
                        }
                    });
                }
            });
        });
    }
}
