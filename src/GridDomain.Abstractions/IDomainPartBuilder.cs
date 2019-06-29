using System.Threading.Tasks;

namespace GridDomain.Abstractions
{
    public interface IDomainPartBuilder
    {
        Task<IDomainPart> Build();
    }
}