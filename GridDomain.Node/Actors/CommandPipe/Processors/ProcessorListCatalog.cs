using System;
using System.Collections.Generic;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.Processors
{
    internal class ProcessorListCatalog : TypeCatalog<List<IMessageProcessor>, object>,
                                          IProcessorListCatalog
    {
        private static readonly List<IMessageProcessor> EmptyProcessorList = new List<IMessageProcessor>();

        public new IReadOnlyCollection<IMessageProcessor> Get(object evt)
        {
            return base.Get(evt) ?? EmptyProcessorList;
        }

        public override void Add(Type type, List<IMessageProcessor> processor)
        {
            List<IMessageProcessor> list;
            if (Catalog.TryGetValue(type, out list))
                list.AddRange(processor);
            else
                base.Add(type, processor);
        }

        public void Add(Type type, IMessageProcessor processor)
        {
            Add(type, new List<IMessageProcessor> {processor});
        }

        public void Add<U>(IMessageProcessor processor)
        {
            Add(typeof(U), processor);
        }
    }
}