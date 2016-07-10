using System;
using System.Linq.Expressions;
using Akka.IO;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging.Waiting
{

    public class ExpectedMessage<T> : ExpectedMessage
    {
        public ExpectedMessage(int messageCount, string idPropertyName = null, Guid messageId = new Guid()) : base(typeof(T), messageCount, idPropertyName, messageId)
        {
        }
    }

    public class ExpectedMessage
    {
        public ExpectedMessage(Type messageType, int messageCount, string idPropertyName = null, Guid messageId = default(Guid))
        {
            MessageType = messageType;
            MessageCount = messageCount;
            IdPropertyName = idPropertyName;
            MessageId = messageId;

            VerifyIdPropertyName(messageType); 
        }

        private void VerifyIdPropertyName(Type messageType)
        {
            if (string.IsNullOrEmpty(IdPropertyName)) return;
            var propertyInfo = messageType.GetProperty(IdPropertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Cannot find property with name {IdPropertyName} in Type {messageType.Name}");
            if (propertyInfo.PropertyType != typeof (Guid))
                throw new ArgumentException($"Property {IdPropertyName} of type {messageType} should be Guid ");
        }

        public Type MessageType { get; }
        public string IdPropertyName { get; }
        public int MessageCount { get; }
        public Guid MessageId { get; }

        public static ExpectedMessage Once(Type messageType, string idPropertyName , Guid messageId)
        {
            return new ExpectedMessage(messageType, 1,idPropertyName, messageId);
        }

        public static ExpectedMessage[] CommandOnce<T>() where T : ICommand
        {
            return CommandOnce<T>(null,Guid.Empty);
        }
        public static ExpectedMessage[] CommandOnce<TCommand>(string correlationProperty, Guid messageId) where TCommand : ICommand
        {
            return new ExpectedMessage []{Once<TCommand>(correlationProperty,messageId),
                                          Once<ICommandFault<TCommand>>(f => f.Id,messageId)};
        }
      
        public static ExpectedMessage<T> Once<T>(string idPropertyName, Guid messageId)
        {
            return new ExpectedMessage<T>(1, idPropertyName, messageId);
        }

        public static ExpectedMessage<T> Once<T>()
        {
            return Once<T>(string.Empty, Guid.Empty);
        }

        public static ExpectedMessage<T> Once<T>(Expression<Func<T, Guid>> idPropertyNameExpression, Guid messageId)
        {
            return Once<T>(MemberNameExtractor.GetName(idPropertyNameExpression), messageId);
        }
    }
}