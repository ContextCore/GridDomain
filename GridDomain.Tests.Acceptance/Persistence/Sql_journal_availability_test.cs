using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Node;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Persistence
{
    [TestFixture]
    public class Sql_journal_availability_test: TestKit
    {
        [Test]
        public void Sql_journal_is_available_for_akka_config()
        {
            var actorSystem = ActorSystem.Create("test", new AutoTestAkkaConfiguration().ToString());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_actor_system()
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration());
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_factory_created_cluster_actor_system()
        {
            var actorSystem = ActorSystemFactory.CreateCluster(new AutoTestAkkaConfiguration(),2,2).RandomElement();
            PingSqlJournal(actorSystem);
        }

        [Test]
        public void Sql_journal_is_available_for_configuraton_created_cluster_actor_system()
        {
            var actorSystem = ActorSystem.Create("local",new AutoTestAkkaConfiguration().ToClusterSeedNodeSystemConfig());
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