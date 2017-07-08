using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface IHandler<in T> 
    {
        Task Handle(T message, IMessageMetadata metadata=null);
    }
}