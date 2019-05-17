using System;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Node.Akka.Actors.Aggregates
{
    public class CommandIdempotencyActor : ReceiveActor
    {
        public CommandIdempotencyActor()
        {
            var writerGuid = Guid.NewGuid().ToString();
            var journal = Persistence.Instance.Apply(Context.System).JournalFor(null);
            IActorRef commandSender = null;
            Receive<CheckCommand>(c =>
            {
                var cmd = c.Command;
                commandSender = Sender;
                var commandPersistentId = AggregateAddress.New(cmd.GetType(), cmd.Id).ToString();

                //protect from repeating commands
                //will try to save event with command Id
                //if persist will fail, need to restart actor and reject the command
                journal.Tell(new WriteMessages(
                    new[]
                    {
                        new AtomicWrite(new Persistent(c.Command,
                            sender: Self,
                            persistenceId: commandPersistentId,
                            sequenceNr: 0,
                            writerGuid: writerGuid))
                    }, Self, 1));
            });
            Receive<WriteMessageSuccess>(s => { });
            Receive<WriteMessagesSuccessful>(s => commandSender.Tell(CommandAccepted.Instance));
            Receive<WriteMessagesFailed>(f => commandSender.Tell(new CommandRejected(f.Cause)));
            Receive<WriteMessageFailure>(f => { });
        }

        public class CheckCommand
        {
            public CheckCommand(ICommand command)
            {
                Command = command;
            }

            public ICommand Command { get; }
        }

        public class CommandAccepted
        {
            private CommandAccepted()
            {
            }

            public static CommandAccepted Instance = new CommandAccepted();
        }

        public class CommandRejected
        {
            public Exception Reason { get; }

            public CommandRejected(Exception reason)
            {
                Reason = reason;
            }
        }
    }
}