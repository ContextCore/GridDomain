using System;
using System.Linq.Expressions;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaHandlerBuilder<TMessage, THandler>:IHandlerBuilder<TMessage,THandler> where THandler : IHandler<TMessage>
    {
        private readonly TypedMessageActor<CreateRoute> _routingRegistrator;
        public string CorrelationPropertyName;
        public AkkaHandlerBuilder(TypedMessageActor<CreateRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public void Register()
        {
            _routingRegistrator.Handle(CreateRoute.New<TMessage, THandler>(CorrelationPropertyName));
        }

        public IHandlerBuilder<TMessage, THandler> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}