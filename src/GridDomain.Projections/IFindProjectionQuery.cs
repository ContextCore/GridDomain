using System.Threading.Tasks;
using GridDomain.Projections;
using GridDomain.Query;

public interface IFindProjectionQuery
{
    Task<Projection> Execute(string name, string projector, string eventName);
}