using System;
using System.Collections.Generic;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    static class ProcessorListCatalog
    {
        internal static readonly List<Processor> EmptyProcessorList = new List<Processor>();
    }

    class ProcessorListCatalog<TMessage> : TypeCatalog<List<Processor>,TMessage>
    {
        public override void Add(Type type, Processor processor)
        {
            List<Processor> list;
            if (!Catalog.TryGetValue(type, out list))
                list = Catalog[type] = new List<Processor>();

            list.Add(processor);
        }

        protected IReadOnlyCollection<Processor> GetProcessor<TMsg>(TMsg message) where TMsg:TMessage
        {
            return base.GetProcessor(message) ?? ProcessorListCatalog.EmptyProcessorList;
        }
    }
}