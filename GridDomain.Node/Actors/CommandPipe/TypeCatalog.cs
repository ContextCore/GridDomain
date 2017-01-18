using System;
using System.Collections.Generic;

namespace GridDomain.Node.Actors.CommandPipe
{
    abstract class TypeCatalog<TData,TMessage>
    {
        protected readonly IDictionary<Type, TData> Catalog = new Dictionary<Type, TData>();

        public abstract void Add(Type type, Processor processor);

        public void Add<U>(Processor processor) where U : TMessage
        {
            Add(typeof(U), processor);
        }

        protected TData GetProcessor(object message)
        {
            TData processor;
            Catalog.TryGetValue(message.GetType(), out processor);
            return processor;
        }
    }
}