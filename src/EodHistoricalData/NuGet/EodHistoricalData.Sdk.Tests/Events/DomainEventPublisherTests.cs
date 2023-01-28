using EodHistoricalData.Sdk.Events;

namespace EodHistoricalData.Sdk.Tests.Events;

public class DomainEventPublisherTests
{
    private readonly List<string> messages;
    private readonly List<string> contexts;

    public DomainEventPublisherTests()
    {
        messages = new();
        contexts = new();
        ApiEventPublisher.RaiseMessageEventHandler += DomainEventPublisher_RaiseMessageEventHandler;
    }

    [Fact]
    public void RaiseMessageEvent_CapturesMessage()
    {
        ApiEventPublisher.RaiseMessageEvent(this, "message", "source", Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.Contains("message", messages);
    }

    [Fact]
    public void RaiseMessageEvent_CapturesContext()
    {
        ApiEventPublisher.RaiseMessageEvent(this, "context", "message", "source", Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.Contains("context", contexts);
    }

    [Fact]
    public void RaiseMessageEvent_DoesNotCaptureEmptyMessage()
    {
        ApiEventPublisher.RaiseMessageEvent(this, "", "source", Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.Empty(messages);
    }

    [Fact]
    public void RaiseMessageEvent_DoesNotCaptureEmptyContext()
    {
        ApiEventPublisher.RaiseMessageEvent(this, "", "message", "source", Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.Empty(contexts);
    }

    [Fact]
    public void RaiseMessageEvent_AllowsNullSender()
    {
        ApiEventPublisher.RaiseMessageEvent(null, "context", "message", "source", Microsoft.Extensions.Logging.LogLevel.Debug);
        Assert.Single(messages);
        Assert.Single(contexts);
    }

    private void DomainEventPublisher_RaiseMessageEventHandler(object? sender, MessageEventArgs e)
    {
        messages.Add(e.Message);
        if (!string.IsNullOrWhiteSpace(e.Context))
        {
            contexts.Add(e.Context);
        }
    }
}
