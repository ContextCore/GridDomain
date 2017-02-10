namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain
{
    class BookOrder_V2 : IOrder
    {
        public BookOrder_V2(string number, int quantity)
        {
            Number = number;
            Quantity = quantity;
        }

        public string Number { get; }
        public int Quantity { get; }
    }
}