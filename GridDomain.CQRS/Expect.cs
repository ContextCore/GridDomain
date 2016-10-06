using System;
using System.Linq.Expressions;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class Expect
    {
        public static ExpectedFault<T> Fault<T>(Expression<Func<T, Guid>> idPropertyNameExpression, Guid messageId, params Type [] source)
        {
            var expectedFault = new ExpectedFault<T>(1, MemberNameExtractor.GetName(idPropertyNameExpression), messageId, source);
            ExpectedMessage.VerifyIdPropertyName(expectedFault.ProcessMessageType, expectedFault.IdPropertyName);
            return expectedFault;
        }

        public static ExpectedMessage<T> Message<T>(string idPropertyName, Guid messageId)
        {
            var expectedMessage = new ExpectedMessage<T>(1, idPropertyName, messageId);
            ExpectedMessage.VerifyIdPropertyName(expectedMessage.MessageType, expectedMessage.IdPropertyName);
            return expectedMessage;
        }

        public static ExpectedMessage<T> Message<T>(Type source = null)
        {
            return new ExpectedMessage<T>(1, null, Guid.Empty, source);
        }

        public static ExpectedMessage Message(Type messageType, string idPropertyName, Guid messageId)
        {
            var expectedMessage = new ExpectedMessage(messageType, 1, idPropertyName, messageId);
            ExpectedMessage.VerifyIdPropertyName(expectedMessage.MessageType, expectedMessage.IdPropertyName);
            return expectedMessage;
        }

        public static ExpectedMessage<T> Message<T>(Expression<Func<T, Guid>> idPropertyNameExpression, Guid messageId)
        {
            return Message<T>(MemberNameExtractor.GetName(idPropertyNameExpression), messageId);
        }
    }
}