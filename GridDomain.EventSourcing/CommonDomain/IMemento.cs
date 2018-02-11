using System;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing.CommonDomain
{
    //must be serializable
    public interface IMemento
    {
        string Id { get; set; }

        int Version { get; set; }
    }
}