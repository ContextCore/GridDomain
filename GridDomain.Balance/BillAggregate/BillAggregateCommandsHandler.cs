using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BillAggregate
{
    class BillAggregateCommandsHandler : AggregateCommandsHandler<Bill>
    {
        public BillAggregateCommandsHandler()
        {
          //  Map<ChargeSubscriptionCommand>(c => c.ChargeId,
          //      c => new Bill(c.ChargeId));
        }
    }
}
