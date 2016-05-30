using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    class InMemory_journal_works: Journal_availability_for_persistent_actor_for_test_akka_system

    {
        public InMemory_journal_works() : base("")
        {
        }

        [Test]
        public void Journal_is_inmemory()
        {
            Assert.AreEqual("akka.persistence.journal.inmem", OnPersistMessage.JournalActorName);
        }
    }
}