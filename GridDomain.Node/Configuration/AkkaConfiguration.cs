using System.Collections.Generic;
using System.Data.Common;

namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public string LogLevel { get; }

        public IAkkaNetworkConfiguration Network { get; }
        public IAkkaDbConfiguration Persistence { get; }

        public AkkaConfiguration(IAkkaNetworkConfiguration networkConf,
                                 IAkkaDbConfiguration dbConf,
                                 LogVerbosity logLevel = LogVerbosity.Warning)
        {
            Network = networkConf;
            Persistence = dbConf;
            _logLevel = logLevel;
            LogLevel = _akkaLogLevels[logLevel];
        }

        public AkkaConfiguration Copy(int newPort)
        {
            var networkConf = Network;
            var network = new AkkaNetworkConfiguration(
                                                       networkConf.Host,
                                                       networkConf.Name,
                                                       newPort);

            return new AkkaConfiguration(network, Persistence, _logLevel);

        }

        private readonly Dictionary<LogVerbosity, string> _akkaLogLevels = new Dictionary<LogVerbosity, string>
        {
                                                                {LogVerbosity.Info, "INFO"},
                                                                {LogVerbosity.Error, "ERROR"},
                                                                {LogVerbosity.Trace, "DEBUG"},
                                                                {LogVerbosity.Warning, "WARNING"}
                                                            };

        private readonly LogVerbosity _logLevel;

        public enum LogVerbosity
        {
            Warning,
            Error,
            Info,
            Trace
        }

        public string GetConfiguraitonString()
        {
            AkkaConfiguration akkaConf = this;
            string logConfig =
                @"
      stdout-loglevel = " + akkaConf.LogLevel + @"
      loglevel = " + akkaConf.LogLevel + @"
      log-config-on-start = on";

            string actorConfig = @"   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

             serialization-bindings {
                                    ""System.Object"" = wire
             }
             
             provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }
       }";

            string remoteConfig = @"
        remote {
               helios.tcp {
                          transport -class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                          transport-protocol = tcp
                          port = " + akkaConf.Network.PortNumber + @"
               }
               hostname = " + akkaConf.Network.Name + @"
        }";

            string persistenceJournalConfig = @"
            journal {
                    plugin = ""akka.persistence.journal.sql-server""
                    sql-server {
                               class = ""Akka.Persistence.SqlServer.Journal.SqlServerJournal, Akka.Persistence.SqlServer""
                              # plugin-dispatcher = ""akka.actor.default-dispatcher""
                               connection-string =  """ + akkaConf.Persistence.JournalConnectionString + @"""
                             #  connection-timeout = 30s
                               schema-name = dbo
                              # table-name = """ + akkaConf.Persistence.JournalTableName + @"""
                               auto-initialize = on
                              # timestamp-provider = ""Akka.Persistence.Sql.Common.Journal.DefaultTimestampProvider, Akka.Persistence.Sql.Common""
                              # metadata-table-name = """ + akkaConf.Persistence.MetadataTableName + @"""
                    }
            }";

            string persistenceSnapshotStorageConfig = @" 
            snapshot-store {
                           plugin =  ""akka.persistence.snapshot-store.sql-server""
                           sql-server {
                                      class = ""Akka.Persistence.SqlServer.Snapshot.SqlServerSnapshotStore, Akka.Persistence.SqlServer""
                                      #plugin-dispatcher = ""akka.actor.default-dispatcher""
                                      connection-string = """ + akkaConf.Persistence.SnapshotConnectionString + @"""
                                      #connection-timeout = 30s
                                      schema-name = dbo
                                    #  table-name = """ + akkaConf.Persistence.SnapshotTableName + @"""
                                      auto-initialize = on
                           }
            }";

            string akkaPersistenceConfig =
                @"      persistence {
                    publish-plugin-commands = on
" + persistenceJournalConfig + @"
" + persistenceSnapshotStorageConfig + @"
        }";

            var hoconConfig = @"akka {  
" + logConfig + @"
" + actorConfig + @"
" + remoteConfig + @"
" + akkaPersistenceConfig + @"
}";
            return hoconConfig;
        }

        public override string ToString()
        {
            return GetConfiguraitonString();
        }
    }


}