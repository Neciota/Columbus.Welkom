using PdxLib.Connector;

namespace Columbus.Welkom.Application.Venira.Rows;

/// <summary>
/// A row of Venira's <c>NEUTTIJD.DB</c>: the NPO sunrise/sunset table that drives NF-14
/// neutralization. The dates are stored against a fixed reference year, so only the month and
/// day are meaningful.
/// </summary>
public sealed record VeniraSolarPeriodRow(
    DateTime? Date,
    TimeSpan? SunUp,
    TimeSpan? SunDown)
{
    public static VeniraSolarPeriodRow FromRecord(ParadoxRecord record) => new(
        Date: record.GetDate("Datum"),
        SunUp: record.GetTime("ZonOp"),
        SunDown: record.GetTime("ZonOnder"));
}
