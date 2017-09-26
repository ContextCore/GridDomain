using System;
using System.Linq;

namespace GridDomain.Node.Configuration.Hocon
{
    internal class RootConfig : INodeConfig
    {
        private readonly INodeConfig[] _parts;

        public RootConfig(params INodeConfig[] parts)
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