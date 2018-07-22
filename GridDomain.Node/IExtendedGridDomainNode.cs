using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node {
    public interface IExtendedGridDomainNode : IGridDomainNode
    {
        ActorSystem System { get; }
        TimeSpan DefaultTimeout { get; }
        IActorTransport Transport { get; }
        IActorCommandPipe Pipe { get; }
        Task<IGridDomainNode> Start();
        Task Stop();
        ILogger Log { get; }
        EventsAdaptersCatalog EventsAdaptersCatalog { get; }
    }
}