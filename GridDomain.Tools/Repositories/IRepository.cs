using System;
using System.Threading.Tasks;

namespace GridDomain.Tools.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        Task Save(string aggregateId, params T[] messages);
        Task<T[]> Load(string persistenceId);
    }
}