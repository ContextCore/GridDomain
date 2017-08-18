using System;
using System.Configuration;
using System.Reflection;
using Akka.Actor;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit;

namespace GridDomain.Tests.Acceptance.Snapshots {
    public static class NodeTestFixtureExtensions
    {
        public static T UseSqlPersistence<T>(this T fixture,bool clearData = true) where T: NodeTestFixture
        {
            fixture.AkkaConfig.Persistence = new AutoTestAkkaDbConfiguration();
            if(clearData)
                fixture.OnNodePreparingEvent += (s, e) =>
                                                {
                                                    TestDbTools.ClearData(e.AkkaConfig.Persistence).Wait();
                                                };

            fixture.SystemConfigFactory = () => fixture.AkkaConfig.ToStandAloneSystemConfig();

            return fixture;

        }
    }
}