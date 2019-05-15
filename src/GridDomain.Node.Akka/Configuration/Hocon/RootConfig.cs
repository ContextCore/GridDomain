using System;
using System.Linq;
using Akka.Configuration;

namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public class RootConfig : IHoconConfig
    {
        private readonly IHoconConfig[] _parts;

        public RootConfig(params IHoconConfig[] parts)
        {
            _parts = parts;
        }

        public string Build()
        {
            var cfg = ByStringParse();
            return cfg;
        }

        private string ByStringParse()
        {
            var configs = _parts.Select(p => p.Build()
                                              .ToString())
                                .ToArray();


            var configString = string.Join(Environment.NewLine, configs);
            return configString;
        }

        private string ByFallbacks()
        {
            return  _parts.Select(p => p.Build())
                         .Aggregate(Config.Empty, (a, c) => a.WithFallback(c)).ToString();
        }
    }
}