using NUnit.Framework;

namespace GridGomain.Tests.Stress
{
    [TestFixture]
    public class RunTest
    {
        [Test]
        public void Run()
        {
            Program.Main();
        }
    }
}