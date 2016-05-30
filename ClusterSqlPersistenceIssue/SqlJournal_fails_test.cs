using Akka.Persistence.SqlServer;
using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    class SqlJournal_fails_test : Journal_availability_for_persistent_actor_for_test_akka_system
    {
        private static readonly string SqlPersistenceConfiguration = @"
akka.persistence{
                journal {
                  plugin = ""akka.persistence.journal.sql-server""
                  sql-server {
                      class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                      schema-name = dbo
                      auto-initialize = on
                      connection-string = ""Data Source=(LocalDB)\\v11.0;Initial Catalog=AutoTestAkka;Integrated Security= True""
                  }
                }

                snapshot-store{
                    plugin = ""akka.persistence.snapshot-store.sql-server""
                    sql-server {
                        class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                        schema-name = dbo
                        auto-initialize = on
                          connection-string = ""Data Source=(LocalDB)\\v11.0;Initial Catalog=AutoTestAkka;Integrated Security= True""
                    }
                }
}";
        [Test]
        public void Journal_is_sql()
        {
            Assert.AreEqual("akka.persistence.journal.sql-server", OnPersistMessage.JournalActorName);
        }

        public SqlJournal_fails_test() : base(SqlPersistenceConfiguration)
        {
          
        }
    }
}