using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class ExpectedMessageBox
    {
        public ExpectedMessageBox(IMessagesExpectation expectation)
        {
            _conditionExpectation = expectation;
        }

        private readonly ConcurrentBag<object> _allExpectedMessages = new ConcurrentBag<object>();
        private readonly IMessagesExpectation _conditionExpectation;

        public bool Receive(object message)
        {
            if (!_conditionExpectation.IsExpected(message)) return false;

            _allExpectedMessages.Add(message);
            return true;
        }

        public bool AllExpectedMessagesReceived()
        {
            return _conditionExpectation.IsExpectationFulfilled(_allExpectedMessages);
        }

        public IReadOnlyCollection<object> ReceivedMessages => _allExpectedMessages;
    }
}