using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    [SetUpFixture]
    public class TestsClobalConfig
    {
        [SetUp]
        public void InitLoggers()
        {
            //InitLog(LogLevel.Error);
            InitLog();
        }

        //public static void InitLog(LogLevel minLevel)
        public static void InitLog()
        {
            //var logConfigurator = new LogConfigurator();
            //logConfigurator.InitConsole(minLevel);

            //logConfigurator.InitDbLogging(minLevel,
            //    TestEnvironment.Configuration.LogsConnectionString);

            ////  logConfigurator.InitExternalLoggin(LogLevel.Trace);
            //logConfigurator.Apply();

            //var logger = LogManager.GetCurrentClassLogger();
            //logger.Info("Logging subsystem configured");
        }
    }
}