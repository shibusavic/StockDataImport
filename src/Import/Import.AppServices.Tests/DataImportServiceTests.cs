//using EodHistoricalData.Sdk;
//using Import.Infrastructure.Abstractions;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Import.AppServices.UnitTests
//{
//    public partial class DataImportServiceTests
//    {

//        private DataImportService CreateDataImportService(
//            out Mock<ILogsDbContext> logsDbMock,
//            out Mock<IImportsDbContext> importsDbMock,
//            out Mock<IDataClient> dataClientMock)
//        {
//            logsDbMock = new Mock<ILogsDbContext>();
//            importsDbMock = new Mock<IImportsDbContext>();
//            dataClientMock = new Mock<IDataClient>();

//            return new DataImportService(logsDbMock.Object,
//                importsDbMock.Object,
//                dataClientMock.Object, 100000, null);
//        }

//    }
//}
