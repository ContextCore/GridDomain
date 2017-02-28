namespace GridDomain.Node.Actors.CommandPipe
{
    public class Initialized
    {
        private Initialized() {}

        public static Initialized Instance { get; } = new Initialized();
    }
}