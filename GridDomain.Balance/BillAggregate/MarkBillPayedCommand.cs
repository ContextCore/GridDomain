using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.BillAggregate
{
    internal class MarkBillPayedCommand : Command
    {
        public MarkBillPayedCommand(Guid id, Guid billId) : base(id)
        {
        }

        public Guid BillId { get; }
    }
}