using System.Threading.Tasks;

namespace GridDomain.EventHandlers
{
    public interface IEventHandler<TEvent>
    {
        Task Handle(Sequenced<TEvent>[] evt);
    }
}