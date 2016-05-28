using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit.NUnit;
using GridDomain.Node;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class Sql_journal_availability_for_cluster_actor_test : TestKit
    {
        private readonly AutoTestAkkaConfiguration _conf = new AutoTestAkkaConfiguration();
        class SqlJournalPingActor : ReceivePersistentActor
        {
            List<string> Events = new List<string>(); 

            public SqlJournalPingActor()
            {
                Command<SqlJournalPing>(m =>
                {
                    Events.Add(m.Payload);
                    Persist(m.Payload, e => Sender.Tell(new Persisted() {Payload = e}));
                });
                Recover<SnapshotOffer>(offer => Events = (List<string>)offer.Snapshot);
            }

            public override string PersistenceId => "test";
        }

        class SqlJournalPing
        {
            public string Payload;
        }
        class Persisted
        {
            public string Payload;
        }

        [Test]
        public void Sql_journal_is_available_for_akka_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.Name, _conf.ToClusterSeedNodeSystemConfig());
            var actor = actorSystem.ActorOf<SqlJournalPingActor>();
            CHeckPersist(actor);
        }

        private void CHeckPersist(IActorRef actor)
        {
            var sqlJournalPing = new SqlJournalPing() {Payload = "testPayload"};
            actor.Tell(sqlJournalPing);
            ExpectMsg<Persisted>(m => m.Payload == sqlJournalPing.Payload, TimeSpan.FromSeconds(100));
        }

        [Test]
        public void Sql_journal_is_available_for_factored_akka_system()
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(_conf);
            var actor = actorSystem.ActorOf<SqlJournalPingActor>();
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_factored_akka_cluster()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(_conf,2,2).RandomElement();
            var actor = actorSystem.ActorOf<SqlJournalPingActor>();
            CHeckPersist(actor);
        }
    }

    [TestFixture]
    public class Sql_journal_availability_test: TestKit
    {
        private readonly AutoTestAkkaConfiguration _conf = new AutoTestAkkaConfiguration();

        [Test]
        public void Sql_journal_is_available_for_akka_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.Name, _conf.ToClusterNonSeedNodeSystemConfig());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_configuraton_created_cluster_actor_system()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.Name, _conf.ToClusterSeedNodeSystemConfig());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_actor_system()
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(_conf);
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_cluster_actor_system()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(_conf, 2,2).RandomElement();
            PingSqlJournal(actorSystem);
        }

      

        [Test]
        public void Sql_journal_is_available_for_testKit_actor_system()
        {
            PingSqlJournal(Sys);
        }

        private static void PingSqlJournal(ActorSystem actorSystem)
        {
            var plugin = Akka.Persistence.Persistence.Instance.Apply(actorSystem).JournalFor(null);
            plugin.Tell(new object());
        }
    }
}