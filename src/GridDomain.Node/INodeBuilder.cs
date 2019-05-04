using System;
using Akka.Actor;
using GridDomain.Domains;
using Microsoft.Extensions.Logging;

namespace GridDomain.Node {
    public interface INodeBuilder {
        //IExtendedGridDomainNode Build();
        INodeBuilder ActorSystem(Func<ActorSystem> sys);
        INodeBuilder Initialize(Action<ActorSystem> sys);
       // IGridNodeBuilder Transport(Action<ActorSystem> sys);
        INodeBuilder Log(ILogger log);
        ILogger Logger { get; }
        INodeBuilder Domains(params IDomainConfiguration[] domainConfigurations);
        INodeBuilder Timeout(TimeSpan timeout);
    }
}