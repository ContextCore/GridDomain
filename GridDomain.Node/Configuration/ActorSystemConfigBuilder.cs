using System.Collections.Generic;
using System.Linq;
using Akka.Configuration;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration.Hocon;
using Serilog;

namespace GridDomain.Node.Configuration
{
    public interface IActorSystemConfigBuilder
    {
        void Add(IHoconConfig cfg);
        Config Build();
        ActorSystemConfigBuilder Clone();
        IActorSystemFactory BuildActorSystemFactory(string systemName);
        ILogger Logger { get; }
    }

    public class ActorSystemConfigBuilder : IActorSystemConfigBuilder
    {
        public ILogger Logger { get; }

        public ActorSystemConfigBuilder(ILogger log = null)
        {
            Logger = log ?? Serilog.Log.Logger;
        }

        private List<IHoconConfig> Configs { get; set; } = new List<IHoconConfig>();

        public static ActorSystemConfigBuilder New(ILogger log = null)
        {
            return new ActorSystemConfigBuilder(log);
        }

        public void Add(IHoconConfig cfg)
        {
            Configs.Add(cfg);
        }

        public Config Build()
        {
            var hocon = new RootConfig(Configs.ToArray());
            return hocon.Build();
        }

        public ActorSystemConfigBuilder Clone()
        {
            return new ActorSystemConfigBuilder(Logger) {Configs = Configs.ToList()};
        }

        public IActorSystemFactory BuildActorSystemFactory(string systemName)
        {
            var hocon = new RootConfig(Configs.ToArray());
            var factory = new HoconActorSystemFactory(systemName, hocon.Build());
            return factory;
        }
    }
}