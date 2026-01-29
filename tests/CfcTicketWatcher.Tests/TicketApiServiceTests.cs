using System.Net;
using CfcTicketWatcher.Functions.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CfcTicketWatcher.Tests;

public class TicketApiServiceTests
{
    private readonly Mock<ILogger<TicketApiService>> _loggerMock;
    private readonly Mock<IConfiguration> _configMock;

    public TicketApiServiceTests()
    {
        _loggerMock = new Mock<ILogger<TicketApiService>>();
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["TicketApiUrl"])
            .Returns("https://webapi.gc.celticfc.com/v1/pages/byfullpath?fullPath=tickets");
    }

    [Fact]
    public async Task GetTicketDataAsync_WithSuccessfulResponse_ReturnsContent()
    {
        // Arrange
        var expectedContent = "{\"success\":true,\"message\":\"OK\",\"body\":{}}";
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedContent);
        var sut = new TicketApiService(httpClient, _configMock.Object, _loggerMock.Object);

        // Act
        var result = await sut.GetTicketDataAsync();

        // Assert
        result.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GetTicketDataAsync_WithFailedResponse_ThrowsException()
    {
        // Arrange
        var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "Error");
        var sut = new TicketApiService(httpClient, _configMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetTicketDataAsync());
    }

    [Fact]
    public async Task GetTicketDataAsync_UsesDefaultUrlWhenNotConfigured()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["TicketApiUrl"]).Returns((string?)null);
        
        var expectedContent = "{\"success\":true}";
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedContent);
        var sut = new TicketApiService(httpClient, configMock.Object, _loggerMock.Object);

        // Act
        var result = await sut.GetTicketDataAsync();

        // Assert
        result.Should().Be(expectedContent);
    }

    [Fact]
    public async Task GetTicketDataAsync_CanBeCancelled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK, "{}", true);
        var sut = new TicketApiService(httpClient, _configMock.Object, _loggerMock.Object);

        // Act & Assert
        // TaskCanceledException inherits from OperationCanceledException
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
            sut.GetTicketDataAsync(cts.Token));
    }

    private static HttpClient CreateMockHttpClient(
        HttpStatusCode statusCode, 
        string content,
        bool delay = false)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        
        var setup = handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        if (delay)
        {
            setup.Returns(async (HttpRequestMessage _, CancellationToken ct) =>
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(1000, ct);
                return new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                };
            });
        }
        else
        {
            setup.ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
        }

        return new HttpClient(handlerMock.Object);
    }
}
