using System;
using System.Linq;
using Akka.Configuration;
using Akka.Configuration.Hocon;

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
            var configStrings = _parts.Select(p => p.Build()).ToArray();
            var configString = string.Join(Environment.NewLine, configStrings.ToString());
            return @"akka {
" + configString + @"
}";
        }
    }
}