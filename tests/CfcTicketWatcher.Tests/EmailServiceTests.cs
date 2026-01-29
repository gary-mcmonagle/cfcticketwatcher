using CfcTicketWatcher.Functions.Services;
using CfcTicketWatcher.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CfcTicketWatcher.Tests;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _loggerMock;

    public EmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailService>>();
    }

    [Fact]
    public async Task SendEmailAsync_WithMissingConnectionString_ReturnsFalse()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AzureCommunicationServicesConnectionString"]).Returns((string?)null);
        configMock.Setup(c => c["NotificationEmailFrom"]).Returns("from@test.com");
        configMock.Setup(c => c["NotificationEmailTo"]).Returns("to@test.com");

        var sut = new EmailService(configMock.Object, _loggerMock.Object);
        var message = CreateTestEmailMessage();

        // Act
        var result = await sut.SendEmailAsync(message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendEmailAsync_WithMissingFromEmail_ReturnsFalse()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AzureCommunicationServicesConnectionString"])
            .Returns("endpoint=https://test.communication.azure.com/;accesskey=test");
        configMock.Setup(c => c["NotificationEmailFrom"]).Returns((string?)null);
        configMock.Setup(c => c["NotificationEmailTo"]).Returns("to@test.com");

        var sut = new EmailService(configMock.Object, _loggerMock.Object);
        var message = CreateTestEmailMessage();

        // Act
        var result = await sut.SendEmailAsync(message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendEmailAsync_WithMissingToEmail_ReturnsFalse()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AzureCommunicationServicesConnectionString"])
            .Returns("endpoint=https://test.communication.azure.com/;accesskey=test");
        configMock.Setup(c => c["NotificationEmailFrom"]).Returns("from@test.com");
        configMock.Setup(c => c["NotificationEmailTo"]).Returns((string?)null);

        var sut = new EmailService(configMock.Object, _loggerMock.Object);
        var message = CreateTestEmailMessage();

        // Act
        var result = await sut.SendEmailAsync(message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendEmailAsync_WithAllEmptyConfiguration_ReturnsFalse()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["AzureCommunicationServicesConnectionString"]).Returns(string.Empty);
        configMock.Setup(c => c["NotificationEmailFrom"]).Returns(string.Empty);
        configMock.Setup(c => c["NotificationEmailTo"]).Returns(string.Empty);

        var sut = new EmailService(configMock.Object, _loggerMock.Object);
        var message = CreateTestEmailMessage();

        // Act
        var result = await sut.SendEmailAsync(message);

        // Assert
        result.Should().BeFalse();
    }

    private static EmailMessage CreateTestEmailMessage()
    {
        return new EmailMessage
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Test Body</p>",
            PlainTextBody = "Test Body",
            MatchId = "g12345",
            QueuedAt = DateTimeOffset.UtcNow
        };
    }
}
