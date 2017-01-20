using System;
using System.Threading.Tasks;

namespace GridDomain.Tools.Repositories
{
    public interface IRepository<T>:IDisposable
    {
        Task Save(string id, params T[] messages);
        T[] Load(string id);
    }
}