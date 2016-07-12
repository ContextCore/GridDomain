using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    //TODO: add execution track status
    public interface IGridDomainNode
    {
        void Execute(params ICommand[] commands);
     
        Task<object> Execute(ICommand command, ExpectedMessage[] expect);
    }
}