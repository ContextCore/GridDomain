namespace GridDomain.Domain.Tests.ProcessingWait
{
    class MessageOfType<T> : IStopCondition
    {
        public bool IsMeet(object msg)
        {
            return msg != null && msg.GetType() == typeof(T);
        }
    }
}