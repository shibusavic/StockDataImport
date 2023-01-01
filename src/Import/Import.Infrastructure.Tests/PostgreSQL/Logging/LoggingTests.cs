using Import.Infrastructure.IntegrationTests.Fixtures;
using Import.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Logging;

public class LoggingTests : IClassFixture<DbFixture>
{
    private readonly DbFixture fixture;

    public LoggingTests(DbFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task SaveLogItem_SavesAsync()
    {
        var sut = fixture.LogsDbContext;

        string scope = Guid.NewGuid().ToString()[0..8];

        int beforeCount = await fixture.GetLogCountForScopeAsync(scope);
        int expectedCount = beforeCount + 1;

        var logItem = new LogItem(LogLevel.Debug,
            Guid.NewGuid().ToString(),
            new Exception(Guid.NewGuid().ToString()),
            scope,
            new EventId(100, "one hundred"),
            new Dictionary<string, string>() {
                {"Function",nameof(SaveLogItem_SavesAsync) }
            });

        await sut.SaveLogAsync(logItem);

        var actualCount = await fixture.GetLogCountForScopeAsync(scope);

        Assert.Equal(expectedCount, actualCount);

        var data = (await fixture.GetDataForLogItemAsync(logItem.GlobalId)).ToArray();
        Assert.NotNull(data);
        Assert.NotEmpty(data);
        Assert.Single(data);
        Assert.Equal("Function", data.First().Key);
        Assert.Equal(nameof(SaveLogItem_SavesAsync), data.First().Value);
    }

    [Fact]
    public async Task PurgeLogs_DeletesAll()
    {
        await CreateLogsAsync(DateTime.Now.AddYears(-1), null, 1, 3);

        var sut = fixture.LogsDbContext;

        var logIds = (await sut.GetAllLogIdsAsync()).ToArray();

        Assert.NotEmpty(logIds);

        await sut.PurgeLogsAsync();

        logIds = (await sut.GetAllLogIdsAsync()).ToArray();

        Assert.Empty(logIds);
    }

    [Fact]
    public async Task PurgeActionLogs_DeletesAll()
    {
        await CreateActionLogsAsync(DateTime.Now.AddYears(-1));

        var sut = fixture.LogsDbContext;

        var count = await sut.CountActionLogsAsync();

        Assert.NotEqual(0, count);

        await sut.PurgeActionLogsAsync();

        count = await sut.CountActionLogsAsync();

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task TruncateLogs_RespectsDate()
    {
        await CreateLogsAsync(DateTime.Now.AddYears(-1), null, 1, 3);
        DateTime targetDate = DateTime.UtcNow.AddMonths(-4);

        var sut = fixture.LogsDbContext;

        var logIds = (await sut.GetAllLogsIdsBeforeDateAsync(targetDate)).ToArray();

        Assert.NotEmpty(logIds);

        await sut.TruncateLogsAsync(LogLevel.Debug.ToString(), targetDate);

        logIds = (await sut.GetAllLogsIdsBeforeDateAsync(targetDate)).ToArray();

        Assert.Empty(logIds);
    }

    private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    private async Task CreateLogsAsync(DateTime startDate, DateTime? endDate = null,
        int minPerDay = 0, int maxPerDay = 5)
    {
        endDate ??= DateTime.UtcNow.AddDays(-1);

        DateTime runner = startDate;

        var db = fixture.LogsDbContext;

        while (runner < endDate)
        {
            int num = Random.Next(minPerDay, maxPerDay + 1);

            for (int i = 0; i < num; i++)
            {
                await db.SaveLogAsync(new LogItem(Guid.NewGuid(),
                    LogLevel.Debug,
                    default, Guid.NewGuid().ToString(), null, "TEST", runner, null));
            }

            runner = runner.AddDays(1);
        }
    }

    private async Task CreateActionLogsAsync(DateTime startDate, DateTime? endDate = null,
        int minPerDay = 0, int maxPerDay = 5)
    {
        endDate ??= DateTime.UtcNow.AddDays(-1);

        DateTime runner = startDate;

        var db = fixture.LogsDbContext;

        while (runner < endDate)
        {
            int num = Random.Next(minPerDay, maxPerDay + 1);

            for (int i = 0; i < num; i++)
            {
                await db.SaveActionItemAsync(new Domain.ActionItem(Guid.NewGuid(),
                    1, "Import", Abstractions.ImportActionStatus.NotStarted,
                    runner, null, null, "Test", "Full", null, null));
            }

            runner = runner.AddDays(1);
        }
    }

}