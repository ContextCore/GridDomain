using Akka.Actor;
using Akka.Configuration;

namespace GridDomain.Node.Configuration
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