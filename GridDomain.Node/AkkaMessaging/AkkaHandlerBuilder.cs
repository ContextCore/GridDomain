using System;
using System.Linq.Expressions;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class AkkaHandlerBuilder<TMessage, THandler>:
        IHandlerBuilder<TMessage,THandler> where THandler : IHandler<TMessage>
    {
        private readonly IHandler<CreateRoute> _routingRegistrator;
        public string CorrelationPropertyName;
        public AkkaHandlerBuilder(IHandler<CreateRoute> routingRegistrator)
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

    public class AkkaAggregateCommandsBuilder<TMessage, TAggregate> :
       IAggregateCommandRouteBuilder<TMessage, TAggregate> where TAggregate : AggregateBase
    {
        private readonly IHandler<CreateActorRoute> _routingRegistrator;
        public string CorrelationPropertyName;
        public AkkaAggregateCommandsBuilder(IHandler<CreateActorRoute> routingRegistrator)
        {
            _routingRegistrator = routingRegistrator;
        }

        public void Register()
        {
            _routingRegistrator.Handle(CreateActorRoute.New<TMessage, TAggregate>());
        }

        public IAggregateCommandRouteBuilder<TMessage, TAggregate> WithCorrelation(string name)
        {
            CorrelationPropertyName = name;
            return this;
        }
    }
}