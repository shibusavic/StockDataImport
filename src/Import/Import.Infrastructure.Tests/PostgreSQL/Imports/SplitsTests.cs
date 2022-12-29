using Import.Infrastructure.IntegrationTests.Fixtures;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Imports;

[Collection("Integration Tests")]
public class SplitsTests : TestBase
{
    public SplitsTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SaveSplits()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var splits = CreateSplits(num).ToArray();

        var beforeCount = await sut.CountSplitsAsync();
        await sut.SaveSplitsAsync(splits);
        var afterCount = await sut.CountSplitsAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    private IEnumerable<Domain.Split> CreateSplits(int numberToCreate = 1)
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            string ratio = i % 2 == 0 ? "3:1" : "2:1";
            string name = Guid.NewGuid().ToString().ToUpper().Replace("-", "");
            yield return new Domain.Split(name[..4], "NYSE", DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)), ratio);
        }
    }
}
