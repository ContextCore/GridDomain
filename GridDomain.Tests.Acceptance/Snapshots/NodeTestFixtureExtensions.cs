using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit;
using Serilog.Events;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public static class NodeTestFixtureExtensions
    {
        public static T SetLogLevel<T>(this T fixture, LogEventLevel value)where T:NodeTestFixture
        {
            fixture.NodeConfig.LogLevel = value;
            return fixture;
        }
        
        public static T UseSqlPersistence<T>(this T fixture, bool clearData = true, ISqlNodeDbConfiguration dbConfig = null) where T : NodeTestFixture
        {
            var persistence = dbConfig ?? new AutoTestNodeDbConfiguration();
            if (clearData)
                fixture.ClearDomainData(dbConfig);

            fixture.ConfigBuilder = n => n.ToDebugStandAloneSystemConfig(persistence);

            return fixture;
        }

        public static T ClearDomainData<T>(this T fixture, ISqlNodeDbConfiguration dbConfig = null) where T : NodeTestFixture
        {
            var persistence = dbConfig ?? new AutoTestNodeDbConfiguration();
            fixture.OnNodePreparingEvent += (s, e) => ClearDomainData(persistence).Wait();

            return fixture;
        }
        
        public static T ClearQuartzPersistence<T>(this T fixture, string connection) where T : NodeTestFixture
        {
                fixture.OnNodePreparingEvent += (s, e) =>
                                                {
                                                    ClearQuartzData(connection).Wait();
                                                };

            return fixture;
        }

        public static async Task ClearQuartzData(string connectionString)
        {
            await TestDbTools.Delete(connectionString,
                                       "QRTZ_FIRED_TRIGGERS",
                                       "QRTZ_SIMPLE_TRIGGERS",
                                       "QRTZ_SIMPROP_TRIGGERS",
                                       "QRTZ_CRON_TRIGGERS",
                                       "QRTZ_BLOB_TRIGGERS",
                                       "QRTZ_TRIGGERS",
                                       "QRTZ_JOB_DETAILS",
                                       "QRTZ_CALENDARS",
                                       "QRTZ_PAUSED_TRIGGER_GRPS",
                                       "QRTZ_LOCKS",
                                       "QRTZ_SCHEDULER_STATE",
                                       "QRTZ_JOB_LISTENERS",
                                       "QRTZ_TRIGGER_LISTENERS");
        }

        public static Task<GridDomainNode> CreateNode(this NodeTestFixture fixt, string logFile)
        {
            return fixt.CreateNode(new XUnitAutoTestLoggerConfiguration(fixt.Output, fixt.NodeConfig.LogLevel, logFile).CreateLogger());
        }
        public static async Task ClearDomainData(ISqlNodeDbConfiguration nodeConf)
        {
            await TestDbTools.Truncate(nodeConf.SnapshotConnectionString.Replace("\\\\", "\\"), nodeConf.SnapshotTableName);
            await TestDbTools.Truncate(nodeConf.JournalConnectionString.Replace("\\\\", "\\"),
                                       nodeConf.JournalTableName,
                                       nodeConf.MetadataTableName);
        }
    }
}