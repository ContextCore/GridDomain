using System.Threading.Tasks;

namespace GridDomain.Domains {
    public interface IDomainConfiguration
    {
        Task Register(IDomainBuilder builder);
    }
}