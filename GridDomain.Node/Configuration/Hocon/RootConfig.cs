using System;
using System.Linq;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Configuration.Hocon
{
    public class RootConfig : IHoconConfig
    {
        private readonly IHoconConfig[] _parts;

        public RootConfig(params IHoconConfig[] parts)
        {
            _parts = parts;
        }

        public Config Build()
        {
            var cfg = ByStringParse();
           // var cfg = ByFallbacks();
            return cfg;
        }

        private Config ByStringParse()
        {
            var configs = _parts.Select(p => p.Build()
                                              .ToString())
                                .ToArray();


            var configString = string.Join(Environment.NewLine, configs);
            Config cfg = configString;
            return cfg;
        }

        private Config ByFallbacks()
        {
            return _parts.Select(p => p.Build())
                         .Aggregate(Config.Empty, (a, c) => a.WithFallback(c));
        }
    }
}