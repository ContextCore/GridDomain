using System;

namespace GridDomain.Tools.Repositories
{
    public interface IEventRepository : IDisposable
    {
        void Save(string id, params object[] messages);
        object[] Load(string id);
    }
}