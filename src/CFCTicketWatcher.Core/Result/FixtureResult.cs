namespace CFCTicketWatcher.Core.Result;

public record class FixtureResult
{
    public DateTime Date { get; init; }
    public string Opponent { get; init; } = string.Empty;
    public string Venue { get; init; } = string.Empty;

}
