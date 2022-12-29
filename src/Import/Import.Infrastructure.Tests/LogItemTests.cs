using Import.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.Tests
{
    public class LogItemTests
    {
        [Fact]
        public void Ctor_Message_Exception_Or_Event_Required()
        {
            Assert.Throws<ArgumentException>(() => new LogItem(LogLevel.None,
                null, null, "TEST", default, null));
        }
    }
}
