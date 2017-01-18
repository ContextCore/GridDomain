using System;
using System.Collections.Generic;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    class ProcessorListCatalog<TMessage> : TypeCatalog<List<Processor>,TMessage>
    {
        public override void Add(Type type, Processor processor)
        {
            List<Processor> list;
            if (!Catalog.TryGetValue(type, out list))
                list = Catalog[type] = new List<Processor>();

            list.Add(processor);
        }
        protected new IReadOnlyCollection<Processor> GetProcessor<U>(U message) where U:TMessage
        {
            return base.GetProcessor(message);
        }
    }
}