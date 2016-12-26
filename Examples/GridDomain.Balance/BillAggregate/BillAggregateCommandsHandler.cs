using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillAggregateCommandsHandler : AggregateCommandsHandler<Bill>
    {
        public BillAggregateCommandsHandler() : base(null)
        {
            Map<CreateBillCommand>(c => c.BillId,
                c => new Bill(c.BillId, c.Charges));

            Map<MarkBillPayedCommand>(c => c.BillId,
                (cmd, agr) => agr.MarkPaid());
        }
    }
}