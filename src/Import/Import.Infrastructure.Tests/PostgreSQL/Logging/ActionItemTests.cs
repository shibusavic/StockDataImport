using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Tests.Fixtures;

namespace Import.Infrastructure.PostgreSQL.Tests;

public class ActionItemTests : IClassFixture<DbFixture>
{
    private readonly DbFixture fixture;

    public ActionItemTests(DbFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task SaveAndGetActionItem()
    {
        var sut = fixture.LogsDbContext;

        var actionItem = new Domain.ActionItem("Do", "TEST");

        await sut.SaveActionItemAsync(actionItem);

        var fromDb = await sut.GetActionItemAsync(actionItem.GlobalId);

        Assert.Equal(actionItem, fromDb);
    }

    [Fact]
    public async Task UpdateActionItem()
    {
        var sut = fixture.LogsDbContext;

        var actionItem = new Domain.ActionItem("Do", "NYSE");

        await sut.SaveActionItemAsync(actionItem);

        actionItem.Complete();

        await sut.SaveActionItemAsync(actionItem);

        var fromDb = (await sut.GetActionItemsByStatusAsync(ImportActionStatus.Completed)).ToArray();

        Assert.NotNull(fromDb);
        Assert.NotEmpty(fromDb);
        Assert.Contains(actionItem, fromDb);
    }

    [Fact]
    public async Task FetchActionItems_DifferentTypes()
    {
        var sut = fixture.LogsDbContext;

        var actionItem1 = new Domain.ActionItem("One", "NYSE");
        var actionItem2 = new Domain.ActionItem("Two", "NYSE");
        var actionItem3 = new Domain.ActionItem("Thr", "NYSE");
        var actionItem4 = new Domain.ActionItem("Not Started", "NYSE");
        var actionItem5 = new Domain.ActionItem("In Progress", "NYSE");

        await sut.SaveActionItemAsync(actionItem1);
        await sut.SaveActionItemAsync(actionItem2);
        await sut.SaveActionItemAsync(actionItem3);
        await sut.SaveActionItemAsync(actionItem4);

        actionItem1.Complete();
        actionItem2.Error(new Exception("Bad Things"));
        actionItem3.Quit();
        actionItem5.Start();

        await sut.SaveActionItemAsync(actionItem1);
        await sut.SaveActionItemAsync(actionItem2);
        await sut.SaveActionItemAsync(actionItem3);
        await sut.SaveActionItemAsync(actionItem5);

        var fromDb = (await sut.GetActionItemsByStatusAsync(ImportActionStatus.Error
            | ImportActionStatus.UsageRequirementMet | ImportActionStatus.NotStarted
            | ImportActionStatus.InProgress)).ToArray();

        Assert.NotNull(fromDb);
        Assert.NotEmpty(fromDb);

        Assert.DoesNotContain(actionItem1, fromDb); // "Completed" NOT included
        Assert.Contains(actionItem2, fromDb);       // Errors included
        Assert.Contains(actionItem3, fromDb);       // Quit prematurely included
        Assert.Contains(actionItem4, fromDb);       // "Not Started" included
        Assert.Contains(actionItem5, fromDb);       // "In Progress" included
    }


    [Fact]
    public async Task GetActionItemsByStatusAsync_BadStatus_Empty()
    {
        var sut = fixture.LogsDbContext;

        Assert.Empty(await sut.GetActionItemsByStatusAsync(ImportActionStatus.None));
    }
}