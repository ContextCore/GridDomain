using System.Threading.Tasks;
using GridDomain.Routing.MessageRouting;

namespace GridDomain.Routing
{
    public interface IMessageRouteMap
    {
        Task Register(IMessagesRouter router);
    }
}