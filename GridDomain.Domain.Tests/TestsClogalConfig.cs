using GridDomain.Node.Configuration;
using NLog;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    [SetUpFixture]
    public class TestsClogalConfig
    {
        [SetUp]
        public void InitLoggers()
        {
            var logConfigurator = new LogConfigurator();
            logConfigurator.InitConsole(LogLevel.Trace, false);
            logConfigurator.Apply();
        }
    }
}