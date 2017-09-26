using System;
using System.Configuration;
using System.Reflection;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit;

namespace GridDomain.Tests.Acceptance.Snapshots {
    public static class NodeTestFixtureExtensions
    {
        public static T UseSqlPersistence<T>(this T fixture, bool clearData = true, INodeDbConfiguration dbConfig = null) where T: NodeTestFixture
        {
            fixture.NodeConfig.Persistence = dbConfig ?? new AutoTestNodeDbConfiguration();
            if(clearData)
                fixture.OnNodePreparingEvent += (s, e) =>
                                                {
                                                    TestDbTools.ClearData(e.NodeConfig.Persistence).Wait();
                                                };

            fixture.SystemConfigFactory = () => fixture.NodeConfig.ToStandAloneSystemConfig();

            return fixture;

        }
    }
}