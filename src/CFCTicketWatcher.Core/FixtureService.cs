using System.Net.Http.Json;
using CFCTicketWatcher.Core.Domain.Fixture;

namespace CFCTicketWatcher.Core;

public interface IFixtureService
{
    Task<Fixture?> GetFixtureAsync(string matchID, int seasonID, string teamID);
}

public class FixtureService(HttpClient httpClient) : IFixtureService
{
    public async Task<Fixture?> GetFixtureAsync(string matchID, int seasonID, string teamID)
    {
        var url = $"v1/fixtures/opta/getsingle?matchID={Uri.EscapeDataString(matchID)}&seasonID={seasonID}&teamID={Uri.EscapeDataString(teamID)}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Fixture>();
    }
}
