namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain
{
    internal class BookOrder_V2 : IOrder
    {
        public BookOrder_V2(string number, int quantity)
        {
            Number = number;
            Quantity = quantity;
        }

        public int Quantity { get; }

        public string Number { get; }
    }
}