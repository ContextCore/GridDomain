using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration {
    public interface IActorSystemConfigBuilder
    {
        void Add(IHoconConfig cfg);
        Config Build();
        ActorSystemConfigBuilder Clone();
        IActorSystemFactory BuildActorSystemFactory(string systemName);
    }
}