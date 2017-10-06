using System;
using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Node
{
    public class NodeDomainConfiguration
    {
        public IReadOnlyCollection<IDomainConfiguration> DomainConfigurations => _domainConfigurations;
        private readonly List<IDomainConfiguration> _domainConfigurations = new List<IDomainConfiguration>();

        public void Add(IDomainConfiguration conf)
        {
            _domainConfigurations.Add(conf);
        }
    }
}