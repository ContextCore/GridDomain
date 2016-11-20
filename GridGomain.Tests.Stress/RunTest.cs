using NUnit.Framework;

namespace GridGomain.Tests.Stress
{
    [TestFixture]
    [Ignore("Run only manually")]
    public class RunTest
    {
        [Test]
        public void Run()
        {
            Program.Main();
        }
    }
}