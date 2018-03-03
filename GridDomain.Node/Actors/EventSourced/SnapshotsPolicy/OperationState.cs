namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    public class OperationState<T>
    {
        public bool IsCompleted { get; set; }
        public T Data { get; set; }
    }
}