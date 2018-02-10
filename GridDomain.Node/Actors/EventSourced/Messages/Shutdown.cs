namespace GridDomain.Node.Actors.EventSourced.Messages
{
    public static class Shutdown
    {
        public class Request
        {

            public static readonly Request Instance = new Request();

            private Request() { }
        }

        public class Declined
        {
            public static readonly Declined Instance = new Declined();

            private Declined() { }
        }

        public class Complete
        {
            public static readonly Complete Instance = new Complete();

            private Complete() { }
        }
    }
}