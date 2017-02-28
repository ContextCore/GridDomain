using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IHandler<in T>
    {
        Task Handle(T msg);
    }
}