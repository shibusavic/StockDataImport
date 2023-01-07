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
        DomainEventPublisher.RaiseMessageEventHandler += DomainEventPublisher_RaiseMessageEventHandler;
    }

    [Fact]
    public void RaiseMessageEvent_CapturesMessage()
    {
        DomainEventPublisher.RaiseMessageEvent(this, "message", "source");
        Assert.Contains("message", messages);
    }

    [Fact]
    public void RaiseMessageEvent_CapturesContext()
    {
        DomainEventPublisher.RaiseMessageEvent(this, "context", "message", "source");
        Assert.Contains("context", contexts);
    }

    [Fact]
    public void RaiseMessageEvent_DoesNotCaptureEmptyMessage()
    {
        DomainEventPublisher.RaiseMessageEvent(this, "", "source");
        Assert.Empty(messages);
    }

    [Fact]
    public void RaiseMessageEvent_DoesNotCaptureEmptyContext()
    {
        DomainEventPublisher.RaiseMessageEvent(this, "", "message", "source");
        Assert.Empty(contexts);
    }

    [Fact]
    public void RaiseMessageEvent_AllowsNullSender()
    {
        DomainEventPublisher.RaiseMessageEvent(null, "context", "message", "source");
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
