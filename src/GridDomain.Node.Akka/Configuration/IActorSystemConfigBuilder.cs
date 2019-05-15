using Akka.Configuration;
using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka.Configuration {
    public interface IActorSystemConfigBuilder
    {
        IActorSystemConfigBuilder Add(IHoconConfig cfg);
        Config Build();
        ActorSystemConfigBuilder Clone();
    }
}