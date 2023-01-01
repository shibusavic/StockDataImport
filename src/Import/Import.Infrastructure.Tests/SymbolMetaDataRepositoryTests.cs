namespace Import.Infrastructure.Tests
{
    public class SymbolMetaDataRepositoryTests
    {
        [Fact]
        public void FindFirstOrDefault_Invalid_Found()
        {
            SymbolMetaDataRepository.SetItems(new SymbolMetaData[] {
                new SymbolMetaData("TEST")
            });

            var item = SymbolMetaDataRepository.Find(r => r.Code == "BAD").FirstOrDefault();

            Assert.Null(item);
        }


        [Fact]
        public void FindFirstOrDefault_Valid1_Found()
        {
            SymbolMetaDataRepository.SetItems(new SymbolMetaData[] {
                new SymbolMetaData("TEST")
            });

            var item = SymbolMetaDataRepository.Find(r => r.Code == "TEST").FirstOrDefault();

            Assert.NotNull(item);
        }

        [Fact]
        public void FindFirstOrDefault_Valid2_Found()
        {
            SymbolMetaDataRepository.SetItems(new SymbolMetaData[] {
                new SymbolMetaData("TEST","EXCHANGE")
            });

            var item = SymbolMetaDataRepository.Find(r => r.Code == "TEST.EXCHANGE").FirstOrDefault();

            Assert.NotNull(item);
        }

        [Fact]
        public void AddOrUpdate_SameObject_Updates()
        {
            SymbolMetaDataRepository.SetItems(new SymbolMetaData[] {
                new SymbolMetaData("TEST","EXCHANGE")
            });

            var existing = SymbolMetaDataRepository.Find(r => r.Code == "TEST.EXCHANGE").FirstOrDefault();
            var beforeDate = existing?.LastUpdated;
            Assert.NotNull(beforeDate);

            Thread.Sleep(10);

            SymbolMetaDataRepository.AddOrUpdate(existing!);

            existing = SymbolMetaDataRepository.Find(r => r.Code == "TEST.EXCHANGE").FirstOrDefault();
            var afterDate = existing?.LastUpdated;
            Assert.NotNull(afterDate);
            Assert.True(afterDate.GetValueOrDefault() > beforeDate.GetValueOrDefault());
        }

        [Fact]
        public void AddOrUpdate_DiffObject_Updates()
        {
            SymbolMetaDataRepository.SetItems(new SymbolMetaData[] {
                new SymbolMetaData("TEST","EXCHANGE")
            });

            var existing = SymbolMetaDataRepository.Find(r => r.Code == "TEST.EXCHANGE").FirstOrDefault();
            var beforeDate = existing?.LastUpdated;
            Assert.NotNull(beforeDate);

            Thread.Sleep(10);

            SymbolMetaDataRepository.AddOrUpdate(new SymbolMetaData("TEST", "EXCHANGE"));

            existing = SymbolMetaDataRepository.Find(r => r.Code == "TEST.EXCHANGE").FirstOrDefault();
            var afterDate = existing?.LastUpdated;
            Assert.NotNull(afterDate);
            Assert.True(afterDate.GetValueOrDefault() > beforeDate.GetValueOrDefault());
        }
    }
}
