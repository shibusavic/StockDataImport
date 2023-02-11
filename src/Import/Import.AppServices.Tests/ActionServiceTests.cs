using Import.AppServices.Configuration;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Configuration;
using Moq;

namespace Import.AppServices.UnitTests;

public class ActionServiceTests
{
    [Fact]
    public void GetActionItemsFromConfigurationAsync_Purges()
    {
        var text = $@"
{Constants.ConfigurationKeys.DatabasePurge}: 
  - Logs
  - Imports
...
";

        var config = ImportConfiguration.Create(text);

        var importsDbMock = new Mock<IImportsDbContext>();

        var sut = new ActionService(importsDbMock.Object);

        var actionItems = sut.GetSortedActionItems(config);

        Assert.NotNull(actionItems);
        Assert.NotEmpty(actionItems);

        Assert.Contains("Purge", actionItems.Select(a => a.ActionName));
        Assert.Contains("Logs", actionItems.Select(a => a.TargetName));
        Assert.Contains("Imports", actionItems.Select(a => a.TargetName));
    }
}
