using System;
using Akka.Actor;

namespace GridDomain.Node.Tests
{
    public class DelegateActorSystemFactory : IActorSystemFactory
    {
        private readonly Func<ActorSystem> _systemCreator;

        public DelegateActorSystemFactory(Func<ActorSystem> systemCreator)
        {
            _systemCreator = systemCreator;
        }

        public ActorSystem CreateSystem()
        {
            return _systemCreator();
        }
    }
}