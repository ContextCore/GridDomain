using System;
using System.Linq.Expressions;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IHandlerBuilder<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        void Register();
        IHandlerBuilder<TMessage, THandler> WithCorrelation(string propertyName);
    }
}