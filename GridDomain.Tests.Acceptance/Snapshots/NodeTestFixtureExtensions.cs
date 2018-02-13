using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
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
                fixture.OnNodePreparingEvent += (s, e) =>
                                                {
                                                    ClearData(persistence).Wait();
                                                };

            fixture.ConfigBuilder = n => n.ToDebugStandAloneSystemConfig(persistence);

            return fixture;
        }

        public static async Task ClearData(ISqlNodeDbConfiguration nodeConf)
        {
            await TestDbTools.Truncate(nodeConf.SnapshotConnectionString.Replace("\\\\", "\\"), nodeConf.SnapshotTableName);
            await TestDbTools.Truncate(nodeConf.JournalConnectionString.Replace("\\\\", "\\"),
                                       nodeConf.JournalTableName,
                                       nodeConf.MetadataTableName);
        }
    }
}