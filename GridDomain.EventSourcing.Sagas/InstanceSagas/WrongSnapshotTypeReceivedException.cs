using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class WrongSnapshotTypeReceivedException : Exception
    {
        public Type Received { get; }
        public Type Expected { get;}

        public WrongSnapshotTypeReceivedException(Type received, Type expected)
        {
            Received = received;
            Expected = expected;
        }
    }
}