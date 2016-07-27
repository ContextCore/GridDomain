using System;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class AdapterDescriptor
    {
        public AdapterDescriptor(Type @from, Type to)
        {
            From = @from;
            To = to;
        }

        public Type From { get; }
        public Type To { get; }

        public static AdapterDescriptor New<TFrom, TTo>()
        {
            return new AdapterDescriptor(typeof(TFrom), typeof(TTo));
        }
    }
}