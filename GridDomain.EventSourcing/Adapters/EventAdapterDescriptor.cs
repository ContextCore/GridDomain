using System;

namespace GridDomain.EventSourcing.Adapters
{
    public class EventAdapterDescriptor
    {
        public EventAdapterDescriptor(Type @from, Type to)
        {
            From = @from;
            To = to;
        }

        public Type From { get; }
        public Type To { get; }

        public static EventAdapterDescriptor New<TFrom, TTo>()
        {
            return new EventAdapterDescriptor(typeof(TFrom), typeof(TTo));
        }
    }
}