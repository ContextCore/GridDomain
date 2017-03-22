using System;

namespace GridDomain.EventSourcing
{
    public interface IHaveSagaId
    {
        Guid SagaId { get; }
    }
}