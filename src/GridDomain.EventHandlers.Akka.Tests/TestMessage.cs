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
    
    public class AnotherTestMessage
    {
        public AnotherTestMessage(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}