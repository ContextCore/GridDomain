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
        /// <summary>
        /// Represents message that is expected to be received
        /// </summary>
        /// <param name="messageType">Type of expected message</param>
        /// <param name="messageCount">Count of expected messages</param>
        /// <param name="idPropertyName">Nullable, if presented message will be checked for if by this field</param>
        /// <param name="messageId">Id of message to wait for, should be presented if idPropertyName is presented</param>
        /// <param name="source">Type of the source of message, typically used for faults</param>
        public ExpectedMessage(Type messageType, int messageCount, string idPropertyName = null, Guid messageId = default(Guid),  Type source = null)
        {
            if(messageType == null) throw new ArgumentNullException(nameof(messageType));
            MessageType = messageType;
            MessageCount = messageCount;

            IdPropertyName = idPropertyName;
            if(IdPropertyName != null && messageId == null)
                throw new ArgumentNullException(nameof(messageId));
            MessageId = messageId;

            Source = source;
        }

        protected virtual bool TryGetMessageId(object msg, out Guid id)
        {
            id = Guid.Empty;
            if (IdPropertyName == null || msg == null) return false;

            var propertyInfo = msg.GetType().GetProperty(IdPropertyName);
            var value = propertyInfo?.GetValue(msg);
            if (!(value is Guid)) return false;
            id = (Guid) value;
            return true;
        }

        public bool Match(object msg)
        {
            var msgType = msg.GetType();
            if (!MessageType.IsAssignableFrom(msgType)) return false;

            Guid messageId;
            //if cannot determine id but having same type, it is OK
            if (!TryGetMessageId(msg, out messageId)) return true;
             return MessageId == messageId;
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
            var expectedMessage = new ExpectedMessage(messageType, 1, idPropertyName, messageId, faultSources);
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