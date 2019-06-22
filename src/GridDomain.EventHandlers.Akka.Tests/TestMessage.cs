namespace GridDomain.EventHandlers.Akka.Tests
{
    public class TestMessage
    {
        public TestMessage(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}