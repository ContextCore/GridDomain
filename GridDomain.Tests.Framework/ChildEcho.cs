namespace GridDomain.Tests.Framework
{
    internal class ChildEcho
    {
        public object Message { get; set; }

        public ChildEcho(object message)
        {
            Message = message;
        }
    }
}