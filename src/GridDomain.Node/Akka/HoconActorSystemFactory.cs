using Akka.Actor;
using Akka.Configuration;
using GridDomain.Common;

namespace GridDomain.Node.Akka
{    
    public class HoconActorSystemFactory : IActorSystemFactory
    {
        private readonly Config _hoconConfig;
        private readonly string _name;

        public HoconActorSystemFactory(string name, Config hoconConfig)
        {
            _name = name;
            _hoconConfig = hoconConfig;
        }

        public ActorSystem CreateSystem()
        {
            return ActorSystem.Create(_name, _hoconConfig);
        }
    }
}