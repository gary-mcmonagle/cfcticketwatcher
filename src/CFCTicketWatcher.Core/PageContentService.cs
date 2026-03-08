using System.Net.Http.Json;
using CFCTicketWatcher.Core.Domain.PageContent;

namespace CFCTicketWatcher.Core;


public interface IPageContentService
{
    public Task<PageData?> GetPageContentAsync(string fullPath);
}

public class PageContentService(HttpClient httpClient) : IPageContentService
{
    public async Task<PageData?> GetPageContentAsync(string fullPath)
    {
        var response = await httpClient.GetAsync($"v1/pages/byfullpath?fullPath={Uri.EscapeDataString(fullPath)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PageData>();
    }
}
