using System;

namespace GridDomain.Tools.Repositories
{
    public interface IRepository<T>:IDisposable
    {
        void Save(string id, params T[] messages);
        T[] Load(string id);
    }
}