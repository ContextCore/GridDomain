using System;
using System.Collections.Generic;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {
    class SeveralHandlersTypeCatalog<T> : TypeCatalog<List<T>, object>
    {
        private static readonly List<T> EmptyProcessorList = new List<T>();

        public new IReadOnlyCollection<T> Get(object evt)
        {
            return base.Get(evt) ?? EmptyProcessorList;
        }

        public override void Add(Type type, List<T> processor)
        {
            if(Catalog.TryGetValue(type, out var list))
                list.AddRange(processor);
            else
                base.Add(type, processor);
        }

        public void Add(Type type, T processor)
        {
            Add(type, new List<T> { processor });
        }

        public void Add<U>(T processor)
        {
            Add(typeof(U), processor);
        }
    }
}