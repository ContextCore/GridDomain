using System;
using System.Collections.Generic;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    class ProcessorListCatalog : TypeCatalog<List<Processor>,object>, IProcessorListCatalog
    {
        private static readonly List<Processor> EmptyProcessorList = new List<Processor>();

        public override void Add(Type type, List<Processor> processor)
        {
            List<Processor> list;
            if (Catalog.TryGetValue(type, out list))
                list.AddRange(processor);
            else
                base.Add(type, processor);
        }

        public void Add(Type type, Processor processor)
        {
            Add(type, new List<Processor> { processor});
        }
        public void Add<U>(Processor processor)
        {
            Add(typeof(U), processor);
        }

        public new IReadOnlyCollection<Processor> Get(object evt)
        {
            return base.Get(evt) ?? EmptyProcessorList;
        }
    }
}