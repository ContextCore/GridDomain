using System;
using Akka.Actor;

namespace GridDomain.Node.Configuration {
    public class DelegateActorSystemFactory : IActorSystemFactory
    {
        private readonly Func<ActorSystem> _creator;
        private readonly Action<ActorSystem> _onCreation;

        public DelegateActorSystemFactory(Func<ActorSystem> creator, Action<ActorSystem> onCreation = null)
        {
            _onCreation = onCreation;
            _creator = creator;
        }
        public ActorSystem CreateSystem()
        {
            var sys = _creator();
            _onCreation?.Invoke(sys);
            return sys;
        }
    }
}