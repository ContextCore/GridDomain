using System;


namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IMemento
    {
        Guid Id { get; set; }

        int Version { get; set; }
    }
}