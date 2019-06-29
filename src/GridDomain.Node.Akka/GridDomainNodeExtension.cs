using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Abstractions;
using GridDomain.Domains;

namespace GridDomain.Node.Akka
{
    public class GridDomainNodeExtension:IExtension
    {
        private readonly ActorSystem _system;
        private readonly List<IDomainConfiguration> _configurations = new List<IDomainConfiguration>();
        private readonly DomainBuilder _domainBuilder = new DomainBuilder();

        public GridDomainNodeExtension(ActorSystem system)
        {
            _system = system;
        }

        public void Register<T>(IExtension ex) where T : class, IDomainPartBuilder
        {
            _domainBuilder.RegisterPartBuilder(() => (T)ex);
        }
        
        public void Register<T>(Func<ActorSystem,T> domainPartFactory) where T : class, IDomainPartBuilder
        {
            _domainBuilder.RegisterPartBuilder(() => domainPartFactory(_system));
        }

        public void Add(params IDomainConfiguration[] cfg)
        {
            _configurations.AddRange(cfg);
        }
        
        public INode Build()
        {
            _domainBuilder.PreparePartBuilders();
            return new GridDomainNode(_system, _domainBuilder,_configurations.ToArray());
        }
    }
}