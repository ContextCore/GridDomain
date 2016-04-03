using GridDomain.Node.Configuration;

namespace GridDomain.Node
{
    public interface IGridDomainNode
    {
        void Start(IDbConfiguration databaseConfiguration);
        void Stop();
    }
}