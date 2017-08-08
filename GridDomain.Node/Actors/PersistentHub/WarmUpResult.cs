namespace GridDomain.Node.Actors.PersistentHub {
    public class WarmUpResult
    {
        public WarmUpResult(ChildInfo info, bool wasCreated)
        {
            Info = info;
            WasCreated = wasCreated;
        }
        public ChildInfo Info { get; }
        public bool WasCreated { get; }
    }
}