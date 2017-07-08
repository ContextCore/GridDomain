namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class Initialized
    {
        private Initialized() {}

        public static Initialized Instance { get; } = new Initialized();
    }
}