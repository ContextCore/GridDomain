using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillAggregateCommandsHandler : AggregateCommandsHandler<Bill>
    {
        public BillAggregateCommandsHandler()
        {
            Map<CreateBillCommand>(c => c.BillId,
                                   c => new Bill(c.BillId,c.Charges));

            Map<MarkBillPayedCommand>(c => c.BillId,
                                     (cmd, agr) => agr.MarkPaid());
        }
    }
}
