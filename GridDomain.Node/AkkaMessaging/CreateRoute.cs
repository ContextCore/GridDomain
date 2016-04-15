using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class MessageRouteInfo
    {
        public Type MessageType { get; }
        /// <summary>
        /// Name of property in message to use as correlation id.
        /// Property must be Guid type. 
        /// All messages with same correlation id will be processed sequencially
        /// to avoid race conditions or concurrency problems.
        /// Can be null.   
        /// </summary>
        public string MessageCorrelationProperty { get; }

        public MessageRouteInfo(Type msgType, string propName)
        {
            MessageType = msgType;
            MessageCorrelationProperty = propName;
        }
    }
    /// <summary>
    /// Each route will be created via separate router, e.g. messages from different routes will not 
    /// be executed in sequence
    /// </summary>
    public class CreateRoute
    {
        public MessageRouteInfo[] MessagesToRoute { get; }
        public Type HandlerType { get; }


        public CreateRoute(Type handlerType, params MessageRouteInfo[] messagesToRoute)
        {
            MessagesToRoute = messagesToRoute;
            HandlerType = handlerType;

            Check();
        }

        public static CreateRoute New<TMessage, THandler>(string property) where THandler: IHandler<TMessage>
        {
            return new CreateRoute(typeof(THandler), new MessageRouteInfo(typeof(TMessage), property));
        }

        private void Check()
        {
            CheckHandler();
            CheckCorrelationProperty(MessagesToRoute);
        }

        private void CheckHandler()
        {
            foreach (var msgInfo in MessagesToRoute)
            {
                var handlerType = typeof (IHandler<>).MakeGenericType(msgInfo.MessageType);
                if (!handlerType.IsAssignableFrom(HandlerType))
                    throw new BadRoute.InvalidHandlerType(HandlerType, msgInfo.MessageType);
            }
        }

        private void CheckCorrelationProperty(MessageRouteInfo[] msgInfos)
        {
            foreach (var msgInfo in msgInfos)
            {
                if (msgInfo.MessageCorrelationProperty == null) return;

                var property = msgInfo.MessageType.GetProperty(msgInfo.MessageCorrelationProperty);
                if (property == null)
                    throw new BadRoute.CannotFindCorrelationProperty(msgInfo.MessageType, msgInfo.MessageCorrelationProperty);
                if (property.PropertyType != typeof (Guid))
                    throw new BadRoute.IncorrectTypeOfCorrelationProperty(msgInfo.MessageType, msgInfo.MessageCorrelationProperty);
            }
        }
    }
}

namespace GridDomain.Node.AkkaMessaging.BadRoute
{
}