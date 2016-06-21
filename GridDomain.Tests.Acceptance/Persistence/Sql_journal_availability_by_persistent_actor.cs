using System;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Framework.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class Sql_journal_availability_by_persistent_actor : TestKit
    {
        private readonly AutoTestAkkaConfiguration _conf =
            new AutoTestAkkaConfiguration(AkkaConfiguration.LogVerbosity.Warning);

        private void CHeckPersist(IActorRef actor)
        {
            var sqlJournalPing = new SqlJournalPing {Payload = "testPayload"};
            actor.Ask(sqlJournalPing);
            ExpectMsg<Persisted>(m => m.Payload == sqlJournalPing.Payload, TimeSpan.FromSeconds(5));
        }


        [Test]
        public void Sql_journal_is_available_for_akka_cluster_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.SystemName, _conf.ToClusterSeedNodeSystemConfig());
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor),nameof(SqlJournalPingActor));
            CHeckPersist(actor);
        }

        [Test]
        public void Sql_journal_is_available_for_akka_standalone_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.SystemName, _conf.ToStandAloneSystemConfig());
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor), nameof(SqlJournalPingActor));
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_factored_akka_cluster()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(_conf, 2, 2).RandomNode();
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor), nameof(SqlJournalPingActor));
            CHeckPersist(actor);
        }

        [Test]
        public void Sql_journal_is_available_for_factored_standalone_akka_system()
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(_conf);
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor), nameof(SqlJournalPingActor));
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_test_akka_config()
        {
            var actor = Sys.ActorOf(Props.Create<SqlJournalPingActor>(TestActor), nameof(SqlJournalPingActor));
            CHeckPersist(actor);
        }
    }
}