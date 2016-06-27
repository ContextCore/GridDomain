using System;
using GridDomain.EventStore.MSSQL.LogPersistance;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace GridDomain.Node.Configuration.Persistence
{
    public class LogConfigurator
    {
        private readonly LoggingConfiguration _config;

        public LogConfigurator(LoggingConfiguration config)
        {
            _config = config;
        }

        public LogConfigurator() : this(new LoggingConfiguration())
        {
        }

        public void Apply()
        {
            LogManager.Configuration = _config;
            LogManager.Configuration.Reload();
        }

        public void InitExternalLoggin(LogLevel level)
        {
            AddUdpTarget(level, "udp://localhost:878", "Log2ViewTarget");
        }

        private void AddUdpTarget(LogLevel level, string address, string name)
        {
            var log2ViewTarget = new ChainsawTarget {Address = address};
            _config.AddTarget(name, log2ViewTarget);
            var ruleLog2View = new LoggingRule("*", level, log2ViewTarget);
            _config.LoggingRules.Add(ruleLog2View);
        }

        public void InitDbLogging(LogLevel minLevel, string logConnectionString)
        {
            DbPersistTarget.DebugTimer.Start();

            LogContext.DefaultConnectionString = logConnectionString;

            ConfigurationItemFactory.Default.Targets
                .RegisterDefinition("DbPersist", typeof (DbPersistTarget));

            var dbTarget = new DbPersistTarget
            {
                ConnectionString = logConnectionString
            };

            var dbAsync = new AsyncTargetWrapper
            {
                WrappedTarget = dbTarget,
                QueueLimit = 5000,
                OverflowAction = AsyncTargetWrapperOverflowAction.Block
            };

            var trg = dbAsync;
            _config.AddTarget("DbTarget", trg);
            var ruleDb = new LoggingRule("*", minLevel, trg);
            _config.LoggingRules.Add(ruleDb);
        }

        public void InitConsole(LogLevel minLevel, bool async = false)
        {
            var target = CreateConsoleTarget();

            if (async)
                target = CreateAsyncWrapper(target);

            _config.AddTarget("console" + Guid.NewGuid(), target);
            var consoleRule = new LoggingRule("*", minLevel, target);
            _config.LoggingRules.Add(consoleRule);
        }

        private Target CreateConsoleTarget()
        {
            return new ColoredConsoleTarget {Layout = @"TM=${date} TH=${threadid} ${logger} ${message}"};
        }

        private Target CreateAsyncWrapper(Target orig)
        {
            return new AsyncTargetWrapper
            {
                WrappedTarget = orig,
                QueueLimit = 5000,
                OverflowAction = AsyncTargetWrapperOverflowAction.Block
            };
        }


        private void InitFileLogging(LogLevel minLevel)
        {
            var fileTarget = new FileTarget {Layout = @"TM=${date} TH=${threadid} ${logger} ${message}"};
            _config.AddTarget("file" + Guid.NewGuid(), fileTarget);
            fileTarget.FileName = "${basedir}/file.txt";
            var fileAsync = new AsyncTargetWrapper
            {
                WrappedTarget = fileTarget,
                QueueLimit = 5000,
                OverflowAction = AsyncTargetWrapperOverflowAction.Block
            };

            var fileRule = new LoggingRule("*", minLevel, fileAsync);
            _config.LoggingRules.Add(fileRule);
        }
    }
}