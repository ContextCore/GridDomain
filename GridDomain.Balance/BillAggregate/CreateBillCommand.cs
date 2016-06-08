using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BillAggregate
{
    public class CreateBillCommand : Command
    {
        public CreateBillCommand(Charge[] charges, Guid billId)
        {
            Charges = charges;
            BillId = billId;
        }

        public Guid BillId { get; }
        public Charge[] Charges { get; }
    }
}