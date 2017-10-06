using System;
using Akka.Actor;

namespace GridDomain.Node.Configuration {
    public class DelegateActorSystemFactory : IActorSystemFactory
    {
        private readonly Func<ActorSystem> _creator;

        public DelegateActorSystemFactory(Func<ActorSystem> creator)
        {
            _creator = creator;
        }
        public ActorSystem Create()
        {
            return _creator();
        }
    }
}