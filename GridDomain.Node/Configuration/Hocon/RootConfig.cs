using System;
using System.Linq;

namespace GridDomain.Node.Configuration.Hocon
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
            var configStrings = _parts.Select(p => p.Build()).ToArray();
            var configString = string.Join(Environment.NewLine, configStrings);
            return @"akka {
" + configString + @"
}";
        }
    }
}