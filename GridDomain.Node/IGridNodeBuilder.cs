using System;
using Akka.Actor;
using GridDomain.Configuration;
using Serilog;

namespace GridDomain.Node {
    public interface IGridNodeBuilder {
        IExtendedGridDomainNode Build();
        IGridNodeBuilder ActorSystem(Func<ActorSystem> sys);
        IGridNodeBuilder Initialize(Action<ActorSystem> sys);
        IGridNodeBuilder Transport(Action<ActorSystem> sys);
        IGridNodeBuilder Log(ILogger log);
        ILogger Logger { get; }
        IGridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations);
        IGridNodeBuilder Timeout(TimeSpan timeout);
    }
}