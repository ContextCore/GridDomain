using System;
using System.Collections.Generic;

namespace GridDomain.Common
{
    public class TypeCatalog<TData, TMessage> : ICatalog<TData, TMessage>
    {
        //TODO: replace with fast match builder
        protected readonly IDictionary<Type, TData> Catalog = new Dictionary<Type, TData>();

        public virtual TData Get(TMessage message)
        {
            Catalog.TryGetValue(message.GetType(), out var processor);
            return processor;
        }

        public virtual void Add(Type type, TData processor)
        {
            Catalog[type] = processor;
        }

        public void Add<U>(TData processor) where U : TMessage
        {
            Add(typeof(U), processor);
        }
    }
}