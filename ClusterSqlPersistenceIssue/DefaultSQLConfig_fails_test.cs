using System.Runtime.Remoting.Contexts;
using Akka.Persistence;
using Akka.Persistence.SqlServer;
using NUnit.Framework;

namespace ClusterSqlPersistenceIssue
{
    class DefaultSQLConfig_fails_test : Journal_availability_for_persistent_actor_for_test_akka_system
    {
        //Config was extracted from SqlServerPersistence.DefaultConfiguration();
        private static readonly string cfg = @"{
  akka : {
    persistence : {
                  journal : {
                    plugin = ""akka.persistence.journal.sql-server""
                    sql-server : {
                      class : ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                      plugin-dispatcher : akka.actor.default-dispatcher
                      connection-string : 
                      connection-timeout : 30s
                      schema-name : dbo
                      table-name : EventJournal
                      auto-initialize : off
                      timestamp-provider :""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                            }
                    }
        snapshot-store : {
                plugin = ""akka.persistence.snapshot-store.sql-server""
                sql-server : {
                  class : ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                  plugin-dispatcher : akka.actor.default-dispatcher
                  connection-string : 
                  connection-timeout : 30s
                  schema-name : dbo
                  table-name : SnapshotStore
                  auto-initialize : off
                }
              }
    }
  }
}
";
        [Test]
        public void Journal_is_sql()
        {
            Assert.AreEqual("akka.persistence.journal.sql-server", OnPersistMessage.JournalActorName);
        }

        public DefaultSQLConfig_fails_test() : base(cfg)
        {
//            //Trying to do some magic 
//           var persistence = new SqlServerPersistenceProvider();
//           persistence.Apply(Sys);
////# var sqlPersistence = new SqlServerPersistence(Sys);

//            var persistence = Persistence.Instance;
//            persistence.Apply(Sys);
        }
    }
}