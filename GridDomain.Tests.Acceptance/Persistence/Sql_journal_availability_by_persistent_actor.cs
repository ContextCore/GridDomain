using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using Akka.TestKit.NUnit;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{

    class SqlJournalNotPersistentActor : UntypedActor
    {
        List<string> _events = new List<string>();
        private readonly IActorRef _notifyActor;

        public SqlJournalNotPersistentActor(IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            var plugin = Akka.Persistence.Persistence.Instance.Apply(Context.System).JournalFor(null);
            plugin.Tell(new object());
        }

        protected override void OnReceive(object message)
        {
            if (message is SqlJournalPing)
            {
                var m = message as SqlJournalPing;
                //  _events.Add(m.Payload);
                _notifyActor.Tell(new Persisted() { Payload = m.Payload });
            }
        }
    }

    class SqlJournalPingActor : PersistentActor
    {
        List<string> _events = new List<string>();
        private readonly IActorRef _notifyActor;

        public SqlJournalPingActor(IActorRef notifyActor)
        {
            _notifyActor = notifyActor;
            var plugin = Akka.Persistence.Persistence.Instance.Apply(Context.System).JournalFor(null);
            plugin.Tell(new object());
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
        }

        protected override void OnPersistRejected(Exception cause, object @event, long sequenceNr)
        {
            base.OnPersistRejected(cause, @event, sequenceNr);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                _events = (List<string>)((SnapshotOffer)message).Snapshot;
            }
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is SqlJournalPing)
            {
                var m = message as SqlJournalPing;
                _events.Add(m.Payload);
                _notifyActor.Tell(new Persisted() { Payload = m.Payload });
            }
            return true;
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


    [TestFixture]
    public class Sql_journal_availability_by_persistent_actor: TestKit
    {
        private readonly AutoTestAkkaConfiguration _conf = new AutoTestAkkaConfiguration(AkkaConfiguration.LogVerbosity.Warning);
      
       

        [Test]
        public void Sql_journal_is_available_for_test_akka_config()
        {
            var actor = Sys.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_akka_cluster_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.Name, _conf.ToClusterSeedNodeSystemConfig());
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }

        [Test]
        public void Sql_journal_is_available_for_akka_standalone_config()
        {
            var actorSystem = ActorSystem.Create(_conf.Network.Name, _conf.ToStandAloneSystemConfig());
          //  var plugin = Akka.Persistence.Persistence.Instance.Apply(actorSystem).JournalFor(null);
           // plugin.Ask(new object());
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }

        private void CHeckPersist(IActorRef actor)
        {
            var sqlJournalPing = new SqlJournalPing() {Payload = "testPayload"};
            actor.Ask(sqlJournalPing);
            ExpectMsg<Persisted>(m => m.Payload == sqlJournalPing.Payload, TimeSpan.FromSeconds(5));
        }

        [Test]
        public void Sql_journal_is_available_for_factored_standalone_akka_system()
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(_conf);
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }


        [Test]
        public void Sql_journal_is_available_for_factored_akka_cluster()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(_conf,2,2).RandomElement();
            var actor = actorSystem.ActorOf(Props.Create<SqlJournalPingActor>(TestActor));
            CHeckPersist(actor);
        }
    }
}