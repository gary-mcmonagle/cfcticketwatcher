using Azure;
using Azure.Data.Tables;

namespace CFCTicketWatcher.Func.TableStorage;

public class FixtureEntity : ITableEntity
{
    public const string FixturePartitionKey = "CelticFC";

    public string PartitionKey { get; set; } = FixturePartitionKey;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public DateTime Date { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
}
