using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class WrongSnapshotTypeReceivedException : Exception
    {
        public WrongSnapshotTypeReceivedException(Type received, Type expected)
        {
            Received = received;
            Expected = expected;
        }

        public Type Received { get; }
        public Type Expected { get; }
    }
}