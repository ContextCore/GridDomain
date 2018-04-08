using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.CommandPipe;

namespace GridDomain.Tests.Unit.Cluster {
    class EchoShardedActor : ReceiveActor
    {
        public EchoShardedActor()
        {
            var log = Context.GetSeriLogger();
            Receive<IMessageMetadata>(m =>
                                      {
                                          log.Info("Got message {msg}", m);
                                          Sender.Tell(m);
                                      });
               
            Receive<ShardEnvelope>(m =>
                                   {
                                       log.Info("Got message {msg}", m);
                                       Sender.Tell(m);
                                   });
                
            Receive<object>(m =>
                            {
                                log.Info("Got message {msg}", m);
                                Sender.Tell(m);
                            });

        }
    }
}