namespace Columbus.Welkom.Application.Models.ViewModels;

public interface IPoints
{
    ICollection<RacePoints> RacePoints { get; }
    double TotalPoints { get; }
}
