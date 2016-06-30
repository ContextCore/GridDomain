using System;
using System.Collections.Generic;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    class ProjectionGroup: IProjectionGroup
    {
        private readonly IServiceLocator _locator;
        readonly Dictionary<Type, IHandler<object>> _handlers = new Dictionary<Type, IHandler<object>>();

        public ProjectionGroup(IServiceLocator locator)
        {
            _locator = locator;
        }

        public void Add<TMessage, THandler>(string correlationPropertyName ) where THandler : IHandler<TMessage>
        {
            _handlers[typeof (TMessage)] = (IHandler<object>)_locator.Resolve<THandler>();
            _acceptMessages.Add(new MessageRoute(typeof(TMessage), correlationPropertyName));
        }

        public void Project(object message)
        {
            var msgType = message.GetType();
            var handler = _handlers[msgType];
            handler.Handle(message);
        }
        private readonly List<MessageRoute> _acceptMessages = new List<MessageRoute>();
        public IReadOnlyCollection<MessageRoute> AcceptMessages => _acceptMessages;
    }
}