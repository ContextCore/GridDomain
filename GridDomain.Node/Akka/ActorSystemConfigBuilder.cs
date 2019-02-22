using System.Collections.Generic;
using System.Linq;
using Akka.Configuration;
using GridDomain.Node.Akka.Configuration;
using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka
{
    public class ActorSystemConfigBuilder : IActorSystemConfigBuilder
    {
        private List<IHoconConfig> Configs { get; set; } = new List<IHoconConfig>();

        public IActorSystemConfigBuilder Add(IHoconConfig cfg)
        {
            Configs.Add(cfg);
            return this;
        }

        public Config Build()
        {
            var hocon = new RootConfig(Configs.ToArray());
            return hocon.Build();
        }

        public ActorSystemConfigBuilder Clone()
        {
            return new ActorSystemConfigBuilder() {Configs = Configs.ToList()};
        }

        public IActorSystemFactory BuildActorSystemFactory(string systemName)
        {
            var hocon = new RootConfig(Configs.ToArray());
            var factory = new HoconActorSystemFactory(systemName, hocon.Build());
            return factory;
        }
    }
}