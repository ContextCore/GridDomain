using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Akka.IO;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExpectedMessage<T> : ExpectedMessage
    {
        public ExpectedMessage(int messageCount, string idPropertyName = null, Guid messageId = new Guid(), Type source = null) : base(typeof(T), messageCount, idPropertyName, messageId)
        {
        }
    }


    public class ExpectedMessage
    {
        public ExpectedMessage(Type messageType, int messageCount, string idPropertyName = null, Guid messageId = default(Guid),  Type source = null)
        {
            MessageType = messageType;
            MessageCount = messageCount;
            IdPropertyName = idPropertyName;
            MessageId = messageId;
            Source = source;
        }

        protected virtual Guid GetMessageId(object msg)
        {
            var propertyInfo = msg.GetType().GetProperty(IdPropertyName);
            if (propertyInfo == null) return Guid.Empty;
            return (Guid)propertyInfo.GetValue(msg);
        }

        public virtual bool Match(object msg)
        {
            var msgType = msg.GetType();
            if (!MessageType.IsAssignableFrom(msgType))
                return false;

            return MessageId == GetMessageId(msg);
        }

        //To get properties from inherited interfaces also
        private static IEnumerable<PropertyInfo> GetPublicProperties(Type type)
        {
            if (!type.IsInterface)
                return type.GetProperties();

            return (new[] { type })
                   .Concat(type.GetInterfaces())
                   .SelectMany(i => i.GetProperties());
        }

        public Type MessageType { get; }
        public string IdPropertyName { get; }
        public int MessageCount { get; }
        public Guid MessageId { get; }
        public Type Source { get; }

        public static ExpectedMessage Once(Type messageType, string idPropertyName, Guid messageId, Type faultSources)
        {
            var expectedMessage = new ExpectedMessage(messageType, 1,idPropertyName, messageId, faultSources);
            VerifyIdPropertyName(expectedMessage.MessageType, expectedMessage.IdPropertyName);
            return expectedMessage;
        }

        public static ExpectedMessage<T> Once<T>(string idPropertyName, Guid messageId, Type source = null)
        {
            var expectedMessage = new ExpectedMessage<T>(1, idPropertyName, messageId, source);
            VerifyIdPropertyName(expectedMessage.MessageType, expectedMessage.IdPropertyName);
            return expectedMessage;
        }

        public static ExpectedFault<T> Fault<T>(Expression<Func<T, Guid>> idPropertyNameExpression, Guid messageId, Type source = null)
        {
            var expectedFault = new ExpectedFault<T>(1, MemberNameExtractor.GetName(idPropertyNameExpression), messageId, source);
            VerifyIdPropertyName(expectedFault.ProcessMessageType, expectedFault.IdPropertyName);
            return expectedFault;
        }

        public static ExpectedMessage<T> Once<T>()
        {
            var expectedMessage = new ExpectedMessage<T>(1);
            VerifyIdPropertyName(expectedMessage.MessageType, expectedMessage.IdPropertyName);
            return expectedMessage;
        }

        public static ExpectedMessage<T> Once<T>(Expression<Func<T,Guid>>  idPropertyNameExpression, Guid messageId, Type source = null)
        {
            return Once<T>(MemberNameExtractor.GetName(idPropertyNameExpression), messageId, source);
        }

        /// <summary>
        /// Checks message type has public Guid property with passed name
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="idPropertyName"></param>
        public static void VerifyIdPropertyName(Type messageType, string idPropertyName)
        {
            if (string.IsNullOrEmpty(idPropertyName)) return;
            var propertyInfos = GetPublicProperties(messageType).Where(pi => pi.Name == idPropertyName).ToArray();
            if (propertyInfos.Length > 1)
                throw new ArgumentException($"Found more than one property with name {idPropertyName} in Type {messageType.Name} hierarchy");

            var propertyInfo = propertyInfos.FirstOrDefault();
            if (propertyInfo == null)
                throw new ArgumentException($"Cannot find property with name {idPropertyName} in Type {messageType.Name}");
            if (propertyInfo.PropertyType != typeof(Guid))
                throw new ArgumentException($"Property {idPropertyName} of type {messageType} should be Guid");
        }
    }
}