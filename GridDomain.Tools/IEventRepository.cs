using System;
using CommonDomain;
using GridDomain.EventSourcing;

namespace GridDomain.Tools
{
    public interface IEventRepository : IDisposable
    {
        void Save(string id, params object[] messages);
        object[] Load(string id);
    }
}