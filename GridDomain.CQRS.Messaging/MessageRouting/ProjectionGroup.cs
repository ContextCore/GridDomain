using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroup: IProjectionGroup
    {
        private readonly IUnityContainer _locator;
        readonly Dictionary<Type, List<Action<object,IMessageMetadata>>> _handlers = new Dictionary<Type, List<Action<object,IMessageMetadata>>>();
        public IProjectionGroupDescriptor Descriptor = new ProjectionGroupDescriptor();

        public ProjectionGroup(IUnityContainer locator=null)
        {
            _locator = locator;
        }

        public void Add<TMessage, THandler>(string correlationPropertyName)
                                            where THandler : IHandler<TMessage>
                                            where TMessage :class
        {
            List<Action<object, IMessageMetadata>> builderList;

            if (!_handlers.TryGetValue(typeof (TMessage), out builderList))
            {
                builderList = new List<Action<object, IMessageMetadata>>();
                _handlers[typeof(TMessage)] = builderList;
                _handlers[typeof(IMessageMetadataEnvelop<TMessage>)] = builderList;
            }

            builderList.Add(ProjectMessage<TMessage, THandler>);

            if (_acceptMessages.All(m => m.MessageType != typeof(TMessage)))
            {
                _acceptMessages.Add(new MessageRoute(typeof(TMessage), correlationPropertyName));
                _acceptMessages.Add(new MessageRoute(typeof(IMessageMetadataEnvelop<TMessage>), correlationPropertyName));
            }
        }

        private void ProjectMessage<TMessage, THandler>(object msg, IMessageMetadata metadata) where THandler : IHandler<TMessage> where TMessage : class
        {
            var message = msg as TMessage;
            if(message == null)
                throw new UnknownMessageException();

            try
            {
                var handler = _locator.Resolve<THandler>();
                var withMetadata = handler as IHandlerWithMetadata<TMessage>;
                if(withMetadata != null)
                    withMetadata.Handle(message, metadata);
                else handler.Handle(message);
            }
            catch (Exception ex)
            {
                throw new MessageProcessException(typeof(THandler), ex);
            }
        }

        public void Project(object message, IMessageMetadata metadata)
        {
            var msgType = message.GetType();
            foreach(var handler in _handlers[msgType])
                    handler(message, metadata);
        }

        private readonly List<MessageRoute> _acceptMessages = new List<MessageRoute>();
        public IReadOnlyCollection<MessageRoute> AcceptMessages => _acceptMessages;
    }
}