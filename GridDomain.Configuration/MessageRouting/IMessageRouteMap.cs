using System.Threading.Tasks;

namespace GridDomain.Configuration.MessageRouting
{
    public interface IMessageRouteMap
    {
        Task Register(IMessagesRouter router);
    }
}