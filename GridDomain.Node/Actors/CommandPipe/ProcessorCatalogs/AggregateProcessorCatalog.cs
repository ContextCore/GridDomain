using System;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    class AggregateProcessorCatalog : TypeCatalog<Processor,ICommand>, IAggregateProcessorCatalog
    {
        public Processor GetAggregateProcessor(ICommand command)
        {
            return GetProcessor(command);
        }

        public override void Add(Type type,Processor processor)
        {
            Catalog[type] = processor;
        }
    }
}