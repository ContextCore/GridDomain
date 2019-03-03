using System;
using System.Threading.Tasks;

namespace GridDomain.Node
{
    public interface INode : IDisposable
    {
        Task<IDomain> Start();
        string Address { get; }
    }
}