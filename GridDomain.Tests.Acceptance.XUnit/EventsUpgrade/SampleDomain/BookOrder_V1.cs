namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade.SampleDomain
{
    class BookOrder_V1 : IOrder
    {
        public BookOrder_V1(string number)
        {
            Number = number;
        }
        public string Number { get; }
    }
}