using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Autofac.Core;
using GridDomain.Abstractions;
using GridDomain.Domains;
using GridDomain.Node.Akka.Extensions.Aggregates;
using Microsoft.Extensions.Logging;

namespace GridDomain.Node.Akka
{
    public class DomainBuilder : IDomainBuilder
    {
        private IContainer _partBuildersContainer;
        private readonly ContainerBuilder _partContainerBuilder;
        private readonly ContainerBuilder _partBuildersContainerBuilder;
        

        public DomainBuilder()
        {
            _partBuildersContainerBuilder = new ContainerBuilder();;
            _partContainerBuilder = new ContainerBuilder();
        }
        public T GetPart<T>() where T : class, IDomainPartBuilder
        {
            return _partBuildersContainer.Resolve<T>();
        }

        public void RegisterPartBuilder<T>(Func<T> partProvider) where T:IDomainPartBuilder
        {
            _partBuildersContainerBuilder.Register(c => partProvider()).ExternallyOwned().As<T>().As<IDomainPartBuilder>();
        }

        public void PreparePartBuilders()
        {
            _partBuildersContainer = _partBuildersContainerBuilder.Build();
        }
        
        public async Task<IDomain> Build()
        {
            foreach (var partBuilder in _partBuildersContainer.Resolve<IEnumerable<IDomainPartBuilder>>())
            {
                _partContainerBuilder.RegisterInstance(await partBuilder.Build()).AsImplementedInterfaces().AsSelf();
            }
            return new Domain(_partContainerBuilder.Build());
        }
    }

    public class Domain : IDomain
    {
        private readonly IContainer _container;

        public Domain(IContainer container)
        {
            _container = container;
        }
        public T GetPart<T>() where T : IDomainPart
        {
            return _container.Resolve<T>();
        }
    }
}