using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Common {
    public interface ITestGridDomainNode : IExtendedGridDomainNode
    {
        Task<T> LoadAggregateByActor<T>(string id) where T : Aggregate;
        Task<TState> LoadProcess<TState>(string id) where TState : class,IProcessState;
        IProcessManagerExpectationBuilder PrepareForProcessManager(DomainEvent msg, MessageMetadata metadata=null);// where TExpect : class;
        IProcessManagerExpectationBuilder PrepareForProcessManager(IFault msg, MessageMetadata metadata=null);// where TExpect : class;
    }
}