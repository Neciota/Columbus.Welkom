namespace Columbus.Welkom.Application.Models.DocumentModels;

public abstract class BaseDocumentModel
{
    public int ClubId { get; set; }
    public int Year { get; set; }
    public IEnumerable<string> RaceCodes { get; set; } = [];
}
