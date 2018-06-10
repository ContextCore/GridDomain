using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
{
    internal class LocalFilesystemSnapshotConfig : IHoconConfig
    {
        public Config Build()
        {
            return @" 
             akka.persistence.snapshot-store {
                            plugin = ""akka.persistence.snapshot-store.local""
                            local {
                                    class = ""Akka.Persistence.Snapshot.LocalSnapshotStore, Akka.Persistence""
                                    plugin-dispatcher = ""akka.persistence.dispatchers.default-plugin-dispatcher""
                                    stream-dispatcher = ""akka.persistence.dispatchers.default-stream-dispatcher""
                                    dir = LocalSnapshots
                            }
                }";
        }
    }
}