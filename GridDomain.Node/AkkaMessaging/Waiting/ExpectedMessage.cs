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
        }

        public Type MessageType { get; }
        public string IdPropertyName { get; }
        public int MessageCount { get; }
        public Guid MessageId { get; }

        public static ExpectedMessage Once(Type messageType, string idPropertyName = null, Guid messageId = default(Guid))
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

        public static ExpectedMessage Once<T>(Expression<Func<T,object>>  idPropertyNameExpression, Guid messageId = default(Guid))
        {
            return new ExpectedMessage(typeof(T), 1, MemberNameExtractor.GetName(idPropertyNameExpression), messageId);
        }
        public static ExpectedMessage DomainEventOnce<T>(string correlationProperty = null, Guid messageId = default(Guid)) where T:DomainEvent
        {
            return new ExpectedMessage(typeof(T), 1, correlationProperty, messageId);
        }
        public static ExpectedMessage[] CommandOne<T>(string correlationProperty = null, Guid messageId = default(Guid)) where T : ICommand
        {
            return new []{new ExpectedMessage(typeof(T), 1, correlationProperty,messageId), 
                          new ExpectedMessage(typeof(ICommandFault<T>),1,nameof(ICommandFault.Id),messageId)};
        }
    }
}