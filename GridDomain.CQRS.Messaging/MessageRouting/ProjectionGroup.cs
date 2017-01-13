using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroup: IProjectionGroup
    {
        private readonly IUnityContainer _locator;
        readonly Dictionary<Type, List<Func<object,IMessageMetadata,Task>>> _handlers = new Dictionary<Type, List<Func<object,IMessageMetadata,Task>>>();

        public ProjectionGroup(IUnityContainer locator=null)
        {
            _locator = locator;
        }

        public void Add<TMessage, THandler>(string correlationPropertyName)
                                            where THandler : IHandler<TMessage>
                                            where TMessage :class
        {
            List<Func<object, IMessageMetadata,Task>> builderList;

            var messageType = typeof(IMessageMetadataEnvelop<TMessage>);

            if (!_handlers.TryGetValue(typeof (TMessage), out builderList))
            {
                builderList = new List<Func<object, IMessageMetadata,Task>>();
                _handlers[typeof(TMessage)] = builderList;
                _handlers[messageType] = builderList;
            }

            builderList.Add(ProjectMessage<TMessage, THandler>);

            if (_acceptMessages.All(m => m.MessageType != messageType))
            {
                _acceptMessages.Add(new MessageRoute(messageType, correlationPropertyName));
            }
        }

        private async Task ProjectMessage<TMessage, THandler>(object msg, IMessageMetadata metadata) where THandler : IHandler<TMessage> where TMessage : class
        {
            var message = msg as TMessage;
            if(message == null)
                throw new UnknownMessageException();

            try
            {
                var handler = _locator.Resolve<THandler>();
                var withMetadata = handler as IHandlerWithMetadata<TMessage>;
                if(withMetadata != null)
                    await withMetadata.Handle(message, metadata);
                else await handler.Handle(message);
            }
            catch (Exception ex)
            {
                throw new ProjectionGroupMessageProcessException(typeof(THandler), ex);
            }
        }

        public async Task Project(object message, IMessageMetadata metadata)
        {
            var msgType = message.GetType();
            foreach(var handler in _handlers[msgType])
                    await handler(message, metadata);
        }

        private readonly List<MessageRoute> _acceptMessages = new List<MessageRoute>();
        public IReadOnlyCollection<MessageRoute> AcceptMessages => _acceptMessages;
        public Task Handle(object msg)
        {
            return Project(msg, MessageMetadata.Empty());
        }

        public Task Handle(object message, IMessageMetadata metadata)
        {
            return Project(message, metadata);
        }
    }
}