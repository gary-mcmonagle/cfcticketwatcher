using CFCTicketWatcher.Core.Result;

namespace CFCTicketWatcher.Func.TableStorage;

public interface IFixtureTableStorageService
{
    Task<List<FixtureResult>> GetStoredFixturesAsync();
    Task UpdateFixturesAsync(List<FixtureResult> fixtures);
}
