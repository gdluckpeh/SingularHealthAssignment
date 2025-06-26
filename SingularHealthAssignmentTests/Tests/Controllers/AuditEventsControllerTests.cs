using Moq;
using Xunit;
using SingularHealthAssignment.Controllers;
using SingularHealthAssignment.Application.Interfaces;
using SingularHealthAssignment.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class AuditEventsControllerTests
{
    private readonly Mock<IAuditEventRepository> _repoMock = new();
    private readonly Mock<ILogger<AuditEventsController>> _loggerMock = new();

    [Fact]
    public async Task Post_ValidEvent_ReturnsCreatedResult()
    {
        var controller = new AuditEventsController(_repoMock.Object, _loggerMock.Object);
        var input = new CreateAuditEventRequest
        {
            ServiceName = "Test",
            EventType = "TEST.EVENT",
            Payload = "{}"
        };

        var result = await controller.Post(input);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var returnedEvent = Assert.IsType<AuditEvent>(created.Value);
        Assert.Equal("Test", returnedEvent.ServiceName);
        Assert.Equal("TEST.EVENT", returnedEvent.EventType);
    }

    [Fact]
    public async Task Get_ReturnsFilteredEvents()
    {
        // Arrange
        var expectedEvents = new List<AuditEvent>
        {
            new() { ServiceName = "TestService", EventType = "TEST.EVENT" }
        };

        _repoMock.Setup(x => x.GetAllAsync("TestService", "TEST.EVENT", null, null))
                 .ReturnsAsync(expectedEvents);

        var controller = new AuditEventsController(_repoMock.Object, _loggerMock.Object);
        var result = await controller.Get("TestService", "TEST.EVENT", null, null);
        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(ok.Value);
        using var doc = JsonDocument.Parse(json);
        var itemsElement = doc.RootElement.GetProperty("Items");
        var items = JsonSerializer.Deserialize<List<AuditEvent>>(itemsElement.GetRawText());

        Assert.NotNull(items);
        Assert.Single(items);
        Assert.Equal("TestService", items[0].ServiceName);
    }


    [Fact]
    public async Task Replay_LogsEvents()
    {
        var ids = new List<Guid> { Guid.NewGuid() };
        _repoMock.Setup(x => x.GetByIdsAsync(ids)).ReturnsAsync(new List<AuditEvent> {
            new() { Id = ids[0], ServiceName = "Test", EventType = "TEST.EVENT", Payload = "{}" }
        });

        var controller = new AuditEventsController(_repoMock.Object, _loggerMock.Object);
        var result = await controller.Replay(ids);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("Replayed", ok.Value.ToString());
    }
}
