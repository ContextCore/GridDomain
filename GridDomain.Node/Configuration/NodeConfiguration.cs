using System;
using Akka.Actor;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;

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


    public class AkkaConfiguration
    {
        public Type LogActorType { get; set; } = typeof(SerilogLoggerActor);
        public LogLevel LogLevel { get; set; }

        public AkkaConfiguration(INodeNetworkAddress networkConf, ISqlNodeDbConfiguration dbConf, LogLevel logLevel = LogLevel.DebugLevel)
        {
            Network = networkConf;
            Persistence = dbConf;
            LogLevel = logLevel;
        }
        public AkkaConfiguration(INodeNetworkAddress networkConf, LogLevel logLevel = LogLevel.DebugLevel):this(networkConf,null,logLevel)
        {
        }

        public INodeNetworkAddress Network { get; set; }
        public ISqlNodeDbConfiguration Persistence { get; set; }
    }
}