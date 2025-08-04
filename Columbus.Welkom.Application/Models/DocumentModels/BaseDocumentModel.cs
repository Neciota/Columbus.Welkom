namespace Columbus.Welkom.Application.Models.DocumentModels;

public abstract class BaseDocumentModel
{
    public required int ClubId { get; set; }
    public required int Year { get; set; }
    public required string LastRaceName { get; set; }
    public required DateTime LastRaceDate { get; set; }
}
