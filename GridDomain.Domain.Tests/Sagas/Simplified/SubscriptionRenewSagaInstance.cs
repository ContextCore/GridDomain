using System;
using Automatonymous;

namespace GridDomain.Tests.Sagas.Simplified
{
    class SubscriptionRenewSagaInstance : ISagaProgress
    {
        public Guid BusinessId { get; set; }
        public Guid SubscriptionId { get; set; }
        public State CurrentState { get; set; }
    }
}