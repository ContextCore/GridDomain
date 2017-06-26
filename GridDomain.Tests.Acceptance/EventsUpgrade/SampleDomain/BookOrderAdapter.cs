using GridDomain.EventSourcing.Adapters;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain
{
    internal class BookOrderAdapter : ObjectAdapter<BookOrder_V1, BookOrder_V2>
    {
        public override BookOrder_V2 Convert(BookOrder_V1 value)
        {
            return new BookOrder_V2(value.Number, 0);
        }
    }
}