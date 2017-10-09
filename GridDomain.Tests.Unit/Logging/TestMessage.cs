namespace GridDomain.Tests.Unit {
    class TestMessage
    {
        public string Payload { get; }

        public TestMessage(string payload)
        {
            Payload = payload;
        }
    }
}