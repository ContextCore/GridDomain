namespace GridDomain.Node.Actors
{
    public class ShutdownCanceled
    {
        private ShutdownCanceled() {}

        public static ShutdownCanceled Instance { get; } = new ShutdownCanceled();
    }
}