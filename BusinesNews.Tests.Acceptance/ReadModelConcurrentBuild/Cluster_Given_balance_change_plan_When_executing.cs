using System;
using BusinessNews.Node;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace BusinesNews.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Cluster_Given_balance_change_plan_When_executing : Given_balance_change_plan_When_executing
    {
        private AkkaCluster _akkaCluster;

        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            _akkaCluster = ActorSystemFactory.CreateCluster(new AutoTestAkkaConfiguration());
            var unityContainer = CreateUnityContainer(dbConfig);

            return new GridDomainNode(unityContainer,
                new BusinessNewsRouting(),
                TransportMode.Cluster, _akkaCluster.All);
        }

        /// <summary>
        ///     Important than persistence setting are the same as for testing cluster as for test ActorSystem
        /// </summary>
        public Cluster_Given_balance_change_plan_When_executing()
            : base(AkkaConf.Copy("writeModelCheckSystem", 9000)
                .ToStandAloneSystemConfig())
        {
        }

        [TestFixtureTearDown]
        public void Terminate()
        {
            _akkaCluster.Dispose();
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);
        protected override int BusinessNum => 1;
        protected override int ChangesPerBusiness => 1;
    }
}