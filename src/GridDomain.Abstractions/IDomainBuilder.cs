using System.Reflection;
using System.Threading.Tasks;

namespace GridDomain.Abstractions {
    public interface IDomainBuilder
    {
        T GetPart<T>() where T : class, IDomainPartBuilder;
        Task<IDomain> Build();
    }

}