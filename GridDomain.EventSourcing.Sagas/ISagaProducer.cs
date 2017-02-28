using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaProducer<out TSaga> where TSaga : ISagaInstance
    {
        ISagaDescriptor Descriptor { get; }

        //TODO: extract to separate type? 
        IReadOnlyCollection<Type> KnownDataTypes { get; }
        TSaga Create(object data);
    }
}