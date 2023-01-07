using EodHistoricalData.Sdk.Events;

namespace EodHistoricalData.Sdk.Tests.Events;

public class MessageEventArgsTests
{
    [Fact]
    public void EventSetsTimestamp()
    {
        var messageEvent = new MessageEventArgs("message", "source");
        TimeSpan ts = new(DateTime.UtcNow.Ticks - messageEvent.UtcTimestamp.Ticks);
        Assert.True(ts.TotalSeconds < 1D);
    }

    [Fact]
    public void EventSetsMessage()
    {
        var messageEvent = new MessageEventArgs("message", "source");
        Assert.Equal("message", messageEvent.Message);
    }

    [Fact]
    public void EventSetsSource()
    {
        var messageEvent = new MessageEventArgs("message", "source");
        Assert.Equal("source", messageEvent.Source);
    }

    [Fact]
    public void MissingSource_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new MessageEventArgs("message", ""));

    [Fact]
    public void MissingMessage_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new MessageEventArgs("", "source"));

    [Fact]
    public void Context_NullByDefault()
    {
        var messageEvent = new MessageEventArgs("message", "source");
        Assert.Null(messageEvent.Context);
    }

    [Fact]
    public void Context_Sets()
    {
        var messageEvent = new MessageEventArgs("context", "message", "source");
        Assert.Equal("context", messageEvent.Context);
    }
}
