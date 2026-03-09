using Azure.Data.Tables;
using CFCTicketWatcher.Core.Result;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.TableStorage;

public partial class FixtureTableStorageService(
    TableServiceClient tableServiceClient,
    ILogger<FixtureTableStorageService> logger) : IFixtureTableStorageService
{
    private const string TableName = "ActiveFixtures";

    private TableClient GetTableClient() => tableServiceClient.GetTableClient(TableName);

    public async Task<List<FixtureResult>> GetStoredFixturesAsync()
    {
        var tableClient = GetTableClient();
        await tableClient.CreateIfNotExistsAsync();

        var fixtures = new List<FixtureResult>();

        await foreach (var entity in tableClient.QueryAsync<FixtureEntity>(
            filter: $"PartitionKey eq '{FixtureEntity.FixturePartitionKey}'"))
        {
            fixtures.Add(new FixtureResult
            {
                Date = entity.Date,
                Opponent = entity.Opponent,
                Venue = entity.Venue
            });
        }

        logger.LogInformation("Retrieved {Count} stored fixtures from table storage", fixtures.Count);
        return fixtures;
    }

    public async Task UpdateFixturesAsync(List<FixtureResult> fixtures)
    {
        var tableClient = GetTableClient();
        await tableClient.CreateIfNotExistsAsync();

        // Delete all existing fixture entities for this partition (in chunks of 100)
        var existingEntities = tableClient.QueryAsync<FixtureEntity>(
            filter: $"PartitionKey eq '{FixtureEntity.FixturePartitionKey}'",
            select: ["PartitionKey", "RowKey"]);

        var deleteActions = new List<TableTransactionAction>();
        await foreach (var entity in existingEntities)
        {
            deleteActions.Add(new TableTransactionAction(TableTransactionActionType.Delete, entity));
        }

        foreach (var chunk in Chunk(deleteActions, 100))
        {
            await tableClient.SubmitTransactionAsync(chunk);
        }

        if (deleteActions.Count > 0)
        {
            logger.LogInformation("Deleted {Count} stale fixture entities from table storage", deleteActions.Count);
        }

        // Insert new fixture entities (in chunks of 100)
        var insertActions = fixtures.Select(f => new TableTransactionAction(
            TableTransactionActionType.UpsertReplace,
            new FixtureEntity
            {
                RowKey = BuildRowKey(f),
                Date = f.Date,
                Opponent = f.Opponent,
                Venue = f.Venue
            })).ToList();

        foreach (var chunk in Chunk(insertActions, 100))
        {
            await tableClient.SubmitTransactionAsync(chunk);
        }

        if (insertActions.Count > 0)
        {
            logger.LogInformation("Stored {Count} fixture entities to table storage", insertActions.Count);
        }
    }

    private static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> source, int size) =>
        source
            .Select((item, index) => (item, index))
            .GroupBy(x => x.index / size)
            .Select(g => g.Select(x => x.item));

    public static string BuildRowKey(FixtureResult fixture)
    {
        // Replace characters invalid in Azure Table Storage row keys (/, \, #, ?, control chars, whitespace)
        var safeOpponent = InvalidRowKeyCharRegex().Replace(fixture.Opponent, "_");
        return $"{fixture.Date:yyyyMMddHHmm}_{safeOpponent}";
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"[/\\#?\x00-\x1f\x7f\s]")]
    private static partial System.Text.RegularExpressions.Regex InvalidRowKeyCharRegex();
}
