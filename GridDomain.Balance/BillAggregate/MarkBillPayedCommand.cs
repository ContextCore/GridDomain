using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BillAggregate
{
    class MarkBillPayedCommand : Command
    {
        public Guid BillId { get; }

        public MarkBillPayedCommand(Guid id, Guid billId):base(id)
        {
            
        }
    }
}