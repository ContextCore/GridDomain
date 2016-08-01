using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroup: IProjectionGroup
    {
        private readonly IUnityContainer _locator;
        readonly Dictionary<Type, List<Action<object>>> _handlers = new Dictionary<Type, List<Action<object>>>();
        public IProjectionGroupDescriptor Descriptor = new ProjectionGroupDescriptor();

        public ProjectionGroup(IUnityContainer locator)
        {
            _locator = locator;
        }

        public void Add<TMessage, THandler>(string correlationPropertyName)
                                            where THandler : IHandler<TMessage>
                                            where TMessage :class
        {
            List<Action<object>> builderList;

            if (!_handlers.TryGetValue(typeof (TMessage), out builderList))
            {
                builderList = new List<Action<object>>();
                _handlers[typeof (TMessage)] = builderList;
            }
            builderList.Add(ProjectMessage<TMessage, THandler>);

            if (_acceptMessages.All(m => m.MessageType != typeof (TMessage)))
                _acceptMessages.Add(new MessageRoute(typeof(TMessage), correlationPropertyName));
        }

        private void ProjectMessage<TMessage, THandler>(object msg) where THandler : IHandler<TMessage> where TMessage : class
        {
            var message = msg as TMessage;
            if(message == null)
                throw new UnknownMessageException();

            var handler = _locator.Resolve<THandler>();
            handler.Handle(message);
        }

        public void Project(object message)
        {
            var msgType = message.GetType();
            foreach(var handler in _handlers[msgType])
 handler(message);
        }
        private readonly List<MessageRoute> _acceptMessages = new List<MessageRoute>();
        public IReadOnlyCollection<MessageRoute> AcceptMessages => _acceptMessages;
    }
}