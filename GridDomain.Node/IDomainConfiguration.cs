using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.Node {
    public interface IDomainConfiguration
    {
        Task Register(IDomainBuilder builder);
    }
}