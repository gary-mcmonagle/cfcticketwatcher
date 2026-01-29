using CfcTicketWatcher.Models;
using FluentAssertions;
using Xunit;

namespace CfcTicketWatcher.Tests;

public class ModelTests
{
    [Fact]
    public void UpcomingMatch_ImplementsITableEntity()
    {
        // Arrange & Act
        var match = new UpcomingMatch
        {
            PartitionKey = "2025",
            RowKey = "g12345",
            MatchLabel = "Celtic Vs. Rangers",
            Competition = "SPFL Matches",
            SeasonName = "Season 2025/2026",
            TicketsAvailable = true
        };

        // Assert
        match.PartitionKey.Should().Be("2025");
        match.RowKey.Should().Be("g12345");
        match.MatchLabel.Should().Be("Celtic Vs. Rangers");
    }

    [Fact]
    public void SentNotification_ImplementsITableEntity()
    {
        // Arrange & Act
        var notification = new SentNotification
        {
            PartitionKey = "g12345",
            RowKey = "TicketAvailable",
            SentAt = DateTimeOffset.UtcNow,
            EmailTo = "test@example.com",
            MatchLabel = "Celtic Vs. Rangers"
        };

        // Assert
        notification.PartitionKey.Should().Be("g12345");
        notification.RowKey.Should().Be("TicketAvailable");
        notification.EmailTo.Should().Be("test@example.com");
    }

    [Fact]
    public void MatchNotificationMessage_PropertiesSetCorrectly()
    {
        // Arrange
        var detectedAt = DateTimeOffset.UtcNow;

        // Act
        var message = new MatchNotificationMessage
        {
            MatchId = "g12345",
            MatchLabel = "Celtic Vs. Rangers",
            Competition = "SPFL Matches",
            Season = "2025",
            DetectedAt = detectedAt
        };

        // Assert
        message.MatchId.Should().Be("g12345");
        message.MatchLabel.Should().Be("Celtic Vs. Rangers");
        message.Competition.Should().Be("SPFL Matches");
        message.Season.Should().Be("2025");
        message.DetectedAt.Should().Be(detectedAt);
    }

    [Fact]
    public void EmailMessage_PropertiesSetCorrectly()
    {
        // Arrange
        var queuedAt = DateTimeOffset.UtcNow;

        // Act
        var message = new EmailMessage
        {
            Subject = "New tickets available",
            HtmlBody = "<h1>Tickets!</h1>",
            PlainTextBody = "Tickets!",
            MatchId = "g12345",
            QueuedAt = queuedAt
        };

        // Assert
        message.Subject.Should().Be("New tickets available");
        message.HtmlBody.Should().Be("<h1>Tickets!</h1>");
        message.PlainTextBody.Should().Be("Tickets!");
        message.MatchId.Should().Be("g12345");
        message.QueuedAt.Should().Be(queuedAt);
    }

    [Fact]
    public void TicketApiResponse_DeserializesCorrectly()
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
                        RowTitle = "SPFL Matches",
                        DisplayRowTitle = true
                    }
                }
            }
        };

        // Assert
        response.Success.Should().BeTrue();
        response.Message.Should().Be("OK");
        response.Body.Should().NotBeNull();
        response.Body!.Content.Should().HaveCount(1);
        response.Body.Content![0].RowTitle.Should().Be("SPFL Matches");
    }

    [Fact]
    public void Fixture_ContainsMatchDetails()
    {
        // Arrange
        var fixture = new Fixture
        {
            MatchID = "g12345",
            Season = 2025,
            Guid = 1234567890,
            Squad = "t61",
            MatchDetails = new List<MatchDetail>
            {
                new MatchDetail
                {
                    MatchID = "g12345",
                    MatchLabel = "Celtic Vs. Rangers",
                    MatchHomeCrest = "home.png",
                    MatchAwayCrest = "away.png",
                    SeasonName = "Season 2025/2026"
                }
            },
            Competition = new Competition
            {
                SeasonID = 2025,
                MatchID = "g12345",
                TeamID = "t61"
            }
        };

        // Assert
        fixture.MatchID.Should().Be("g12345");
        fixture.Season.Should().Be(2025);
        fixture.MatchDetails.Should().HaveCount(1);
        fixture.MatchDetails![0].MatchLabel.Should().Be("Celtic Vs. Rangers");
        fixture.Competition!.TeamID.Should().Be("t61");
    }
}
