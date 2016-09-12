using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaProducer<out TSaga> where TSaga : ISagaInstance
    {
        TSaga Create(object data);

        ISagaDescriptor Descriptor { get; }

        //TODO: extract to separate type? 
        IReadOnlyCollection<Type> KnownDataTypes { get; }
    }
}