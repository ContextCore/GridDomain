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
            var propertyInfos = GetPublicProperties(messageType).Where(pi => pi.Name == IdPropertyName).ToArray();
            if(propertyInfos.Length > 1)
                throw new ArgumentException($"Found more than one property with name {IdPropertyName} in Type {messageType.Name} hierarchy");

            var propertyInfo = propertyInfos.FirstOrDefault();
            if (propertyInfo == null)
                throw new ArgumentException($"Cannot find property with name {IdPropertyName} in Type {messageType.Name}");
            if (propertyInfo.PropertyType != typeof (Guid))
                throw new ArgumentException($"Property {IdPropertyName} of type {messageType} should be Guid ");
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

        public static ExpectedMessage Once(Type messageType, string idPropertyName , Guid messageId)
        {
            return new ExpectedMessage(messageType, 1,idPropertyName, messageId);
        }

        public static ExpectedMessage<T> Once<T>(string idPropertyName, Guid messageId)
        {
            return new ExpectedMessage<T>(1, idPropertyName, messageId);
        }
        public static ExpectedMessage<T> Once<T>()
        {
            return new ExpectedMessage<T>(1);
        }
        public static ExpectedMessage<T> Once<T>(Expression<Func<T,Guid>>  idPropertyNameExpression, Guid messageId)
        {
            return Once<T>(MemberNameExtractor.GetName(idPropertyNameExpression), messageId);
        }
    }
}