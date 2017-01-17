using System;
using System.Collections.Generic;

namespace GridDomain.Node.Actors.CommandPipe
{
    abstract class TypeCatalog<TData,TMessage>
    {
        protected readonly IDictionary<Type, TData> _catalog = new Dictionary<Type, TData>();

        public abstract void Add<U>(Processor processor) where U : TMessage;

        protected TData GetProcessor<U>(U message) where U:TMessage
        {
            TData processor;
            _catalog.TryGetValue(typeof(U), out processor);
            return processor;
        }
    }
}