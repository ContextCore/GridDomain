using System;
using System.Linq.Expressions;
using Akka.IO;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
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

        public static ExpectedMessage Once<T>(string idPropertyName, Guid messageId)
        {
            return new ExpectedMessage(typeof(T), 1, idPropertyName, messageId);
        }
        public static ExpectedMessage Once<T>()
        {
            return new ExpectedMessage(typeof(T), 1);
        }
        public static ExpectedMessage Once<T>(Expression<Func<T,Guid>>  idPropertyNameExpression, Guid messageId)
        {
            return Once(typeof(T), MemberNameExtractor.GetName(idPropertyNameExpression), messageId);
        }

        public static ExpectedMessage[] CommandOnce<T>() where T : ICommand
        {
            return CommandOnce<T>(null,Guid.Empty);
        }
        public static ExpectedMessage[] CommandOnce<T>(string correlationProperty, Guid messageId) where T : ICommand
        {
            return new []{Once<T>(correlationProperty,messageId),
                          Once<ICommandFault<T>>(f => f.Id,messageId)};
        }
    }
}