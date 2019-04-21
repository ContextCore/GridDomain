using System;
using System.Threading.Tasks;
using GridDomain.Domains;

namespace GridDomain.Node
{
    public interface INode : IDisposable
    {
        Task<IDomain> Start();
        string Address { get; }
    }
}