using System;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class Sql_journal_availability_test : TestKit
    {
        private readonly AutoTestAkkaConfiguration _conf = new AutoTestAkkaConfiguration();

        private static void PingSqlJournal(ActorSystem actorSystem)
        {
            var plugin = Akka.Persistence.Persistence.Instance.Apply(actorSystem).JournalFor(null);
            plugin.Ask(new object(),TimeSpan.FromSeconds(1));
        }

        [Test]
        public void Sql_journal_is_available_for_akka_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.SystemName, _conf.ToClusterNonSeedNodeSystemConfig());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_configuraton_created_cluster_actor_system()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.SystemName, _conf.ToClusterSeedNodeSystemConfig());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_actor_system()
        {
            var actorSystem = _conf.CreateSystem();
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_cluster_actor_system()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(_conf, 2, 2).RandomNode();
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_testKit_actor_system()
        {
            PingSqlJournal(Sys);
        }
    }
}