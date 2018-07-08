using System;
using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public interface IMessagesExpectation
    {
        bool IsExpected(object message);
        bool IsExpectationFulfilled(IReadOnlyCollection<object> messages);
        IReadOnlyCollection<Type> ExpectedMessageTypes { get; }
    }
}