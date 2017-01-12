using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IGenericQuery<TRes>
    {
        Task<TRes> Execute();
    }
}