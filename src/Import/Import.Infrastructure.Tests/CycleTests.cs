using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Import.Infrastructure.Tests
{
    public class CycleTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public CycleTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        
        static readonly Random r = new(Guid.NewGuid().GetHashCode());

        [Fact]
        public void CycleIdCreation()
        {
            const int size = 4;

            Span<byte> bytes = new byte[size];
            r.NextBytes(bytes);

            string rnd = Encoding.UTF8.GetString(bytes);

            string eng = Convert.ToBase64String(bytes);

            string rndEng = Convert.ToBase64String(Encoding.UTF8.GetBytes(rnd));

            testOutputHelper.WriteLine(rnd);
            testOutputHelper.WriteLine(rnd.Length.ToString());
            testOutputHelper.WriteLine(eng);
            testOutputHelper.WriteLine(eng.Length.ToString());
            testOutputHelper.WriteLine(rndEng);
            testOutputHelper.WriteLine(rndEng.Length.ToString());

            Assert.NotEqual(rnd, eng);
        }

        [Fact]
        public void CycleId2Creation()
        {
            const int size = 12;

            for (int i = 0; i < 4; i++)
            {
                var ticks = new string(DateTime.Now.Ticks.ToString().ToCharArray()[^size..^0]);

                Assert.Equal(size, ticks.Length);

                testOutputHelper.WriteLine(ticks);

                Thread.Sleep(1_000);

                Assert.NotNull(ticks);
            }

        }
    }
}
