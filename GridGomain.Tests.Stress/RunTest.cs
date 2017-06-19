using System.Threading.Tasks;
using NUnit.Framework;

namespace GridGomain.Tests.Stress
{
    [TestFixture]
   // [Ignore("Run only manually")]
    public class RunTest
    {
        [Test]
        public async Task Run()
        {
            await Program.RawCommandExecution(10,10,10);
        }
    }
}