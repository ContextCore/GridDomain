using Akka.Actor;

namespace GridDomain.Node.Configuration
{
    public class HoconActorSystemFactory : IActorSystemFactory
    {
        private readonly string _hoconConfig;
        private readonly string _name;

        public HoconActorSystemFactory(string name, string hoconConfig)
        {
            _name = name;
            _hoconConfig = hoconConfig;
        }
        public ActorSystem Create()
        {
            return ActorSystem.Create(_name, _hoconConfig);
        }
    }
}