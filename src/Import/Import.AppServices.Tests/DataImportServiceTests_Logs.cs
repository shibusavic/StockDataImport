//using Import.Infrastructure.Abstractions;
//using Moq;
//using static Import.Infrastructure.Configuration.Constants;

//namespace Import.AppServices.UnitTests;

//public partial class DataImportServiceTests
//{
//    [Fact]
//    public async Task PurgeDataAsync_Logs()
//    {
//        var sut = CreateDataImportService(out Mock<ILogsDbContext> logsDbMock, out _, out _);

//        await sut.PurgeDataAsync(PurgeName.Logs);

//        logsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.EndsWith("eod_logs")), // EndsWith because Contains() catches the other table.
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//        logsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("eod_logs_extended")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));
//    }

//    [Fact]
//    public async Task PurgeDataAsync_ActionLogs()
//    {
//        var sut = CreateDataImportService(out Mock<ILogsDbContext> logsDbMock, out _, out _);

//        await sut.PurgeDataAsync(PurgeName.ActionLogs);

//        logsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("eod_action_logs")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));
//    }

//    [Fact]
//    public async Task PurgeDataAsync_Imports()
//    {
//        var sut = CreateDataImportService(out _, out Mock<IImportsDbContext> importsDbMock, out _);

//        await sut.PurgeDataAsync(PurgeName.Imports);

//        importsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.symbols")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//        importsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.splits")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//        importsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.dividends")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//        importsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.price_actions")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//    }

//    [Fact]
//    public async Task TruncateLogs()
//    {
//        var sut = CreateDataImportService(out Mock<ILogsDbContext> logsDbMock, out _, out _);

//        await sut.TruncateLogsAsync("Critical", DateTime.Now.AddYears(-1));

//        logsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.eod_logs ")), // space at end intentional
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));

//        logsDbMock.Verify(m => m.ExecuteAsync(It.Is<string>(s => s.Contains("public.eod_logs_extended")),
//            It.IsAny<object?>(), It.IsAny<int?>(), CancellationToken.None));
//    }
//}
