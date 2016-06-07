using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class BusinessAggregateCommandsHandler : AggregateCommandsHandler<Business>
    {
        public BusinessAggregateCommandsHandler()
        {
            Map<OrderSubscriptionCommand>(c => c.BusinessId,
                                          (c,a) => a.OrderSubscription(c.SubscriptionId,c.OfferId));

            Map<CompleteBusinessSubscriptionOrderCommand>(c => c.BusinessId,
                                                          (cmd, agr) => agr.PurchaseSubscription(cmd.SubscriptionId));
        }
    }
}
