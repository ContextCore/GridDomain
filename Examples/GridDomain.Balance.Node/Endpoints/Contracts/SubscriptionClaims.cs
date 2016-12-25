using System;
using System.Collections.Generic;

namespace BusinessNews.Node.Endpoints.Contracts
{
    public class SubscriptionClaims
    {
        public Guid SubscriptionId;
        public IReadOnlyCollection<string> Claims;
    }
}