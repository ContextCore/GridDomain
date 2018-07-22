using System;
using Akka.Actor;
using GridDomain.Configuration;
using Serilog;

namespace GridDomain.Node {
    public interface IGridNodeBuilder {
        IExtendedGridDomainNode Build();
        GridNodeBuilder ActorSystem(Func<ActorSystem> sys);
        GridNodeBuilder Initialize(Action<ActorSystem> sys);
        GridNodeBuilder Transport(Action<ActorSystem> sys);
        GridNodeBuilder Log(ILogger log);
        ILogger Logger { get; }
        GridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations);
        GridNodeBuilder Timeout(TimeSpan timeout);
    }
}