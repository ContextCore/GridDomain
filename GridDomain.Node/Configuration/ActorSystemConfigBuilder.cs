using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration.Hocon;
using Serilog;

namespace GridDomain.Node.Configuration {
    public class ActorSystemConfigBuilder
    {
        public readonly ILogger Logger;

        public ActorSystemConfigBuilder(ILogger log=null)
        {
            Logger = log ?? Serilog.Log.Logger;
        }

        public List<IHoconConfig> Configs { get; private set; } = new List<IHoconConfig>();

        public static ActorSystemConfigBuilder New(ILogger log=null)
        {
            return new ActorSystemConfigBuilder(log);
        }
        public void Add(IHoconConfig cfg)
        {
            Configs.Add(cfg);
        }

        public string Build()
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