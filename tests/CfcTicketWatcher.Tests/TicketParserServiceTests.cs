using System.Text.Json;
using CfcTicketWatcher.Functions.Services;
using CfcTicketWatcher.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CfcTicketWatcher.Tests;

public class TicketParserServiceTests
{
    private readonly Mock<ILogger<TicketParserService>> _loggerMock;
    private readonly TicketParserService _sut;

    public TicketParserServiceTests()
    {
        _loggerMock = new Mock<ILogger<TicketParserService>>();
        _sut = new TicketParserService(_loggerMock.Object);
    }

    [Fact]
    public void ParseApiResponse_WithValidJson_ReturnsTicketApiResponse()
    {
        // Arrange
        var json = CreateSampleApiResponse();

        // Act
        var result = _sut.ParseApiResponse(json);

        // Assert
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Message.Should().Be("OK");
        result.Body.Should().NotBeNull();
    }

    [Fact]
    public void ParseApiResponse_WithInvalidJson_ReturnsNull()
    {
        // Arrange
        var json = "{ invalid json }";

        // Act
        var result = _sut.ParseApiResponse(json);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseApiResponse_WithEmptyJson_ReturnsNull()
    {
        // Arrange
        var json = "";

        // Act
        var result = _sut.ParseApiResponse(json);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseUpcomingMatches_WithValidFixtures_ReturnsMatches()
    {
        // Arrange
        var json = CreateSampleApiResponse();

        // Act
        var result = _sut.ParseUpcomingMatches(json);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        
        var firstMatch = result[0];
        firstMatch.RowKey.Should().Be("g2563072");
        firstMatch.PartitionKey.Should().Be("2025");
        firstMatch.MatchLabel.Should().Be("Celtic Vs. Falkirk - Sun, Feb 1st 2026, 15:00");
        firstMatch.Competition.Should().Be("SPFL Matches");
        firstMatch.TicketsAvailable.Should().BeTrue();
    }

    [Fact]
    public void ParseUpcomingMatches_WithEmptyContent_ReturnsEmptyList()
    {
        // Arrange
        var response = new TicketApiResponse
        {
            Success = true,
            Message = "OK",
            Body = new TicketApiBody
            {
                Content = new List<ContentRow>()
            }
        };
        var json = JsonSerializer.Serialize(response);

        // Act
        var result = _sut.ParseUpcomingMatches(json);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseUpcomingMatches_WithNoFixturesWidget_ReturnsEmptyList()
    {
        // Arrange
        var response = new TicketApiResponse
        {
            Success = true,
            Message = "OK",
            Body = new TicketApiBody
            {
                Content = new List<ContentRow>
                {
                    new ContentRow
                    {
                        RowType = "WidgetRow",
                        RowTitle = "Text Widget",
                        RowData = new List<RowDataItem>
                        {
                            new RowDataItem
                            {
                                WidgetType = "TextBlockWidget",
                                WidgetData = new WidgetData
                                {
                                    Content = "<p>Some text</p>"
                                }
                            }
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(response);

        // Act
        var result = _sut.ParseUpcomingMatches(json);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseUpcomingMatches_ExtractsCompetitionFromRowTitle()
    {
        // Arrange
        var json = CreateApiResponseWithCompetition("Europa League Matches");

        // Act
        var result = _sut.ParseUpcomingMatches(json);

        // Assert
        result.Should().HaveCount(1);
        result[0].Competition.Should().Be("Europa League Matches");
    }

    [Fact]
    public void ParseUpcomingMatches_HandlesMultipleCompetitions()
    {
        // Arrange
        var json = CreateMultiCompetitionResponse();

        // Act
        var result = _sut.ParseUpcomingMatches(json);

        // Assert
        result.Should().HaveCount(3);
        result.Select(m => m.Competition).Distinct().Should().HaveCount(2);
    }

    [Fact]
    public void ParseUpcomingMatches_SetsLastUpdatedToCurrentTime()
    {
        // Arrange
        var json = CreateSampleApiResponse();
        var beforeParse = DateTimeOffset.UtcNow;

        // Act
        var result = _sut.ParseUpcomingMatches(json);
        var afterParse = DateTimeOffset.UtcNow;

        // Assert
        result.Should().NotBeEmpty();
        foreach (var match in result)
        {
            match.LastUpdated.Should().BeOnOrAfter(beforeParse);
            match.LastUpdated.Should().BeOnOrBefore(afterParse);
        }
    }

    private static string CreateSampleApiResponse()
    {
        var response = new TicketApiResponse
        {
            Success = true,
            Message = "OK",
            Body = new TicketApiBody
            {
                Content = new List<ContentRow>
                {
                    new ContentRow
                    {
                        RowType = "WidgetRow",
                        RowTitle = "SPFL Matches",
                        RowData = new List<RowDataItem>
                        {
                            new RowDataItem
                            {
                                WidgetType = "FixturesListWidget",
                                WidgetData = new WidgetData
                                {
                                    Fixtures = new List<Fixture>
                                    {
                                        new Fixture
                                        {
                                            MatchID = "g2563072",
                                            Season = 2025,
                                            Guid = 1767721229892,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail
                                                {
                                                    MatchID = "g2563072",
                                                    MatchLabel = "Celtic Vs. Falkirk - Sun, Feb 1st 2026, 15:00",
                                                    MatchHomeCrest = "794c1e80-572a-11f0-a67f-ddadc7c5dfab.png",
                                                    MatchAwayCrest = "8800f7e0-9b4a-4b1c-963e-6f1ae3d7786f.png",
                                                    SeasonName = "Season 2025/2026"
                                                }
                                            }
                                        },
                                        new Fixture
                                        {
                                            MatchID = "g2563078",
                                            Season = 2025,
                                            Guid = 1769177347657,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail
                                                {
                                                    MatchID = "g2563078",
                                                    MatchLabel = "Aberdeen Vs. Celtic - Wed, Feb 4th 2026, 20:00",
                                                    MatchHomeCrest = "3b1ea0ed-5f37-47e9-9274-baad07b977be.png",
                                                    MatchAwayCrest = "794c1e80-572a-11f0-a67f-ddadc7c5dfab.png",
                                                    SeasonName = "Season 2025/2026"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return JsonSerializer.Serialize(response);
    }

    private static string CreateApiResponseWithCompetition(string competition)
    {
        var response = new TicketApiResponse
        {
            Success = true,
            Message = "OK",
            Body = new TicketApiBody
            {
                Content = new List<ContentRow>
                {
                    new ContentRow
                    {
                        RowType = "WidgetRow",
                        RowTitle = competition,
                        RowData = new List<RowDataItem>
                        {
                            new RowDataItem
                            {
                                WidgetType = "FixturesListWidget",
                                WidgetData = new WidgetData
                                {
                                    Fixtures = new List<Fixture>
                                    {
                                        new Fixture
                                        {
                                            MatchID = "g2602077",
                                            Season = 2025,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail
                                                {
                                                    MatchID = "g2602077",
                                                    MatchLabel = "Celtic Vs. FC Utrecht - Thu, Jan 29th 2026, 20:00"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return JsonSerializer.Serialize(response);
    }

    private static string CreateMultiCompetitionResponse()
    {
        var response = new TicketApiResponse
        {
            Success = true,
            Message = "OK",
            Body = new TicketApiBody
            {
                Content = new List<ContentRow>
                {
                    new ContentRow
                    {
                        RowType = "WidgetRow",
                        RowTitle = "SPFL Matches",
                        RowData = new List<RowDataItem>
                        {
                            new RowDataItem
                            {
                                WidgetType = "FixturesListWidget",
                                WidgetData = new WidgetData
                                {
                                    Fixtures = new List<Fixture>
                                    {
                                        new Fixture
                                        {
                                            MatchID = "g1",
                                            Season = 2025,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail { MatchID = "g1", MatchLabel = "Match 1" }
                                            }
                                        },
                                        new Fixture
                                        {
                                            MatchID = "g2",
                                            Season = 2025,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail { MatchID = "g2", MatchLabel = "Match 2" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ContentRow
                    {
                        RowType = "WidgetRow",
                        RowTitle = "Europa League",
                        RowData = new List<RowDataItem>
                        {
                            new RowDataItem
                            {
                                WidgetType = "FixturesListWidget",
                                WidgetData = new WidgetData
                                {
                                    Fixtures = new List<Fixture>
                                    {
                                        new Fixture
                                        {
                                            MatchID = "g3",
                                            Season = 2025,
                                            MatchDetails = new List<MatchDetail>
                                            {
                                                new MatchDetail { MatchID = "g3", MatchLabel = "Match 3" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return JsonSerializer.Serialize(response);
    }
}
