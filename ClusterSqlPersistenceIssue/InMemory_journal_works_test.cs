using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    class InMemory_journal_works_test: Journal_availability_for_persistent_actor_for_test_akka_system

    {
        public InMemory_journal_works_test() : base("")
        {
        }

        [Test]
        public void Journal_is_inmemory()
        {
            Assert.AreEqual("akka.persistence.journal.inmem", OnPersistMessage.JournalActorName);
        }
    }
}