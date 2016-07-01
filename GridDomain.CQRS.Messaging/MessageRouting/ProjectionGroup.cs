using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroup: IProjectionGroup
    {
        private readonly IServiceLocator _locator;
        readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public ProjectionGroup(IServiceLocator locator)
        {
            _locator = locator;
        }

        public void Add<TMessage, THandler>(string correlationPropertyName ) where THandler : IHandler<TMessage>
        {
            var handler = _locator.Resolve<THandler>();
            _handlers[typeof (TMessage)] =  o => handler.Handle((TMessage) o);

            _acceptMessages.Add(new MessageRoute(typeof(TMessage), correlationPropertyName));
        }

        public void Project(object message)
        {
            var msgType = message.GetType();
            var handler = _handlers[msgType];
            handler(message);
        }
        private readonly List<MessageRoute> _acceptMessages = new List<MessageRoute>();
        public IReadOnlyCollection<MessageRoute> AcceptMessages => _acceptMessages;
    }
}