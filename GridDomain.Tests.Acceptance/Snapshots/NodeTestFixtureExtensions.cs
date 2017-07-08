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
        public static T UseSqlPersistence<T>(this T fixture) where T: NodeTestFixture
        {
            fixture.AkkaConfig.Persistence = GetConfig();
            fixture.OnNodePreparingEvent += (s,e) => TestDbTools.ClearData(e.AkkaConfig.Persistence).Wait();
            fixture.OnNodePreparingEvent += (s,e) => e.NodeSettings.QuartzConfig = new PersistedQuartzConfig();
            fixture.SystemConfigFactory = () => fixture.AkkaConfig.ToStandAloneSystemConfig();

            return fixture;

        } 

        private static IAkkaDbConfiguration GetConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var section = (WriteDbConfigSection)config.GetSection("WriteDb");
            return section?.ElementInformation.IsPresent == true ? (IAkkaDbConfiguration)section : new AutoTestAkkaDbConfiguration();
        }
    }
}