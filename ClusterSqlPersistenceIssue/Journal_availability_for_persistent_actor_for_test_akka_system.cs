using System;
using Akka.Actor;
using Akka.Persistence.SqlServer;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    [TestFixture]
    public class Journal_availability_for_persistent_actor_for_test_akka_system : TestKit
    {
        protected Persisted OnPersistMessage;
        private SqlJournalPing _sqlJournalPing;

        public Journal_availability_for_persistent_actor_for_test_akka_system(string config):base(config)
        {
         
        }

        [TestFixtureSetUp]
        public void Given_system_with_sql_persistence()
        {
            var actor = Sys.ActorOf(Props.Create<PersistencePingActor>(TestActor));
            _sqlJournalPing = new SqlJournalPing() { Payload = "testPayload" };
            actor.Ask(_sqlJournalPing);
            OnPersistMessage = ExpectMsg<Persisted>(TimeSpan.FromSeconds(3));
        }

        [Test]
        public void Payload_is_correct()
        {
            Assert.AreEqual(_sqlJournalPing.Payload, OnPersistMessage.Payload);

        }
    }
}