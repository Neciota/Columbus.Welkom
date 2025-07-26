namespace Columbus.Welkom.Application.Models.ViewModels;

public class AppSettings
{
    public int Year { get; set; } = DateTime.Now.Year;
    public int Club { get; set; } = 2151;
    public string AppDirectory { get; set; } = string.Empty;

    public string GetDatabasePath() => Path.Combine(AppDirectory, $"database_{Club}_{Year}.db");
}
