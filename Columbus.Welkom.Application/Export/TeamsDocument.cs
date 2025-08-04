using Columbus.Welkom.Application.Models.DocumentModels;
using Columbus.Welkom.Application.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Columbus.Welkom.Application.Export;

public class TeamsDocument(Teams teams) : BaseDocument(teams)
{
    private readonly Teams _teams = teams;

    protected override string Title => "Ploegenspel";

    protected override PageSize PageSize => PageSizes.A4.Landscape();

    protected override void ComposeContent(IContainer container)
    {
        container.Table(table =>
        {
            int highestPosition = _teams.AllTeams.MaxBy(t => t.TeamOwners.MaxBy(to => to.Position)?.Position ?? 0)?.TeamOwners.MaxBy(to => to.Position)?.Position ?? 0;

            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                for (int positionInTeam = 0; positionInTeam < highestPosition; positionInTeam++)
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(1);
                }
                
            });

            int position = 0;
            foreach (Team team in _teams.AllTeams.OrderByDescending(to => to.TotalPoints))
            {
                position++;
                table.Cell().Text($"{position}.").LineHeight(1.5f);
                table.Cell().Text(team.TotalPoints.ToString()).LineHeight(1.5f);
                for (int positionInTeam = 0; positionInTeam < highestPosition; positionInTeam++)
                {
                    TeamOwner? teamOwner = team.TeamOwners.FirstOrDefault(to => to.Position == positionInTeam);

                    table.Cell().Text(teamOwner?.Owner?.Name).LineHeight(1.5f);
                    table.Cell().Text(teamOwner?.Points.ToString()).LineHeight(1.5f);
                }
            }
        });
    }
}
