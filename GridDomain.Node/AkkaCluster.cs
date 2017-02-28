using System;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node
{
    public class AkkaCluster : IDisposable
    {
        public ActorSystem[] NonSeedNodes;
        public ActorSystem[] SeedNodes;

        public ActorSystem[] All => SeedNodes.Concat(NonSeedNodes)
                                             .ToArray();

        public void Dispose()
        {
            foreach (var actorSystem in All)
            {
                //  Cluster.Get(actorSystem).Down(actorSystem);
                actorSystem.Terminate();
                actorSystem.Dispose();
            }
        }

        public ActorSystem RandomNode()
        {
            return SeedNodes.Concat(NonSeedNodes)
                            .Last();
        }
    }
}