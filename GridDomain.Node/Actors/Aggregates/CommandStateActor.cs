using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Node.Actors.Aggregates
{
    public class CommandStateActor : ReceivePersistentActor
    {
        private CommandState State { get; set; }

        public CommandStateActor()
        {
            State = CommandState.New;
            PersistenceId = Self.Path.Name;
            Command<AcceptCommandExecution>(a =>
                                            {
                                                if (State != CommandState.Executed)
                                                    Sender.Tell(Accepted.Instance);
                                                else
                                                    Sender.Tell(Rejected.Instance);
                                            });
            Command<CommandSucceed>(s => Persist(CommandState.Executed, e => State = e));
            Command<CommandFailed>(s => Persist(CommandState.Failed, e => State = e));
            Recover<CommandState>(s => State = s);
        }

        public override string PersistenceId { get; }

        private enum CommandState
        {
            New,
            Executed,
            Failed
        }

        public class AcceptCommandExecution
        {
            private AcceptCommandExecution() { }
            public static AcceptCommandExecution Instance { get; } = new AcceptCommandExecution();
        }

        public class Accepted
        {
            private Accepted() { }
            public static Accepted Instance { get; } = new Accepted();
        }

        public class Rejected
        {
            private Rejected() { }
            public static Rejected Instance { get; } = new Rejected();
        }

        public class CommandFailed
        {
            private CommandFailed() { }
            public static CommandFailed Instance { get; } = new CommandFailed();
        }

        public class CommandSucceed
        {
            private CommandSucceed() { }
            public static CommandSucceed Instance { get; } = new CommandSucceed();
        }
    }
}