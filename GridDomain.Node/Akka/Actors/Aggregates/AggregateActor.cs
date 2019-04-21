using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;
using Akka.Persistence;
using DotNetty.Common.Utilities;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Node.Akka.AggregatesExtension;
using GridDomain.Node.Akka.Logging;
using GridDomain.Node.Tests;

namespace GridDomain.Node.Akka.Actors.Aggregates
{
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : ReceivePersistentActor where TAggregate : class, IAggregate
    {
        protected override ILoggingAdapter Log { get; } = Context.GetSeriLogger();
        public override string PersistenceId => _persistentId;
        private string _persistentId;
        protected string Id { get; }
        public TAggregate Aggregate { get; private set; }
        private AggregateCommandExecutionContext ExecutionContext { get; } = new AggregateCommandExecutionContext();
        protected readonly BehaviorQueue Behavior;
        private readonly DateTime _startedTime;
        private readonly IActorRef _journal;
        private bool? _isCommandAlreadyExecuted;
        private Guid guid = Guid.NewGuid();
        private readonly IActorRef _commandChecker;


        public AggregateActor()
        {
            Behavior = new BehaviorQueue(Become);
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
            _persistentId = Self.Path.Name;
            Id = AggregateAddress.Parse<TAggregate>(Self.Path.Name).Id;

            var aggregateExtensions = Context.System.GetAggregatesExtension();
            var dependencies = aggregateExtensions.GetDependencies<TAggregate>();
            Aggregate = dependencies.AggregateFactory.Build();

            Context.SetReceiveTimeout(dependencies.Settings.MaxInactivityPeriod);
            Recover<DomainEvent>(e => Aggregate.Apply(e));

            _commandChecker = Context.ActorOf(Props.Create<CommandIdempotentActor>(), "CommandIdempotenceWatcher");
            _startedTime = BusinessDateTime.UtcNow;
        }

        protected virtual void AwaitingCommandBehavior()
        {
            Command<ReceiveTimeout>(t =>
            {
                Context.Parent.Tell(new Passivate(AggregateActor.ShutdownGratefully.Instance));
            });

            Command<AggregateActor.ShutdownGratefully>(s => { Context.Stop(Self); });
            Command<AggregateActor.CheckHealth>(c => Sender.Tell(new AggregateHealthReport(Self.Path.ToString(),
                BusinessDateTime.UtcNow - _startedTime,
                Context.System.GetAddress().ToString())));
            Command<AggregateActor.ExecuteCommand>(m =>
            {
                var cmd = m.Command;
                if (cmd.Recipient.Id != Id)
                    throw new AggregateIdMismatchException();

                Log.Debug("Received command {cmdId}", cmd.Id);

                ExecutionContext.Command = cmd;
                ExecutionContext.CommandMetadata = m.Metadata;
                ExecutionContext.CommandSender = Sender;
                ExecutionContext.IsWaitingForConfirmation = m.IsWaitingAcknowledgement;
              
                try
                {
                    Aggregate.Execute(cmd).PipeTo(Self);
                }
                catch (Exception ex)
                {
                    StopOnException("Aggregate could not produce domain events", ex);
                }

                Behavior.Become(ProcessingCommandBehavior, nameof(ProcessingCommandBehavior));
            });
        }

        protected override void OnPersistFailure(Exception cause, object @event, long sequenceNr)
        {
            ExecutionContext.CommandSender.Tell(new AggregateActor.CommandFailed(cause));
            base.OnPersistFailure(cause, @event, sequenceNr);
        }

        private void ProcessingCommandBehavior()
        {
            Command<IReadOnlyCollection<IDomainEvent>>(domainEvents =>
            {
                Log.Debug("command executed, starting to persist events");

                if (!domainEvents.Any())
                {
                    Log.Warning("Trying to persist events but no events is presented. {@context}", ExecutionContext);
                    return;
                }

                ExecutionContext.ProducedEvents = domainEvents;
                ExecutionContext.EventsPersisted = false;
                
                //check if we already executed this command
                _isCommandAlreadyExecuted = null;
                _commandChecker.Tell(new CommandIdempotentActor.CheckCommand(ExecutionContext.Command));

            });

            Command<CommandIdempotentActor.CommandAccepted>(s =>
            {
                _isCommandAlreadyExecuted = false;
                if (ExecutionContext.ProducedEvents != null && !ExecutionContext.EventsPersisted)
                {
                    PersistProducedEvents(ExecutionContext.ProducedEvents);
                }
            });
            Command<CommandIdempotentActor.CommandRejected>(f =>
            {
                _isCommandAlreadyExecuted = true;
                StopOnException("Command was rejected as already executed", new CommandAlreadyExecutedException());
            });
           
            //aggregate raised an error during command execution
            Command<Status.Failure>(f => { StopOnException("Aggregate command execution timeout or could not produce domain events", f.Cause); });

            CommandAny(StashMessage);
        }

        private void PersistProducedEvents(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            ExecutionContext.EventsPersisted = true;
            
            int messagesToPersistCount = domainEvents.Count;
            PersistAll(domainEvents,
                persistedEvent =>
                {
                    Aggregate.ApplyByVersion(persistedEvent);

                    if (--messagesToPersistCount != 0) return;
                    CompleteExecution();
                });
        }

        private void StopOnException(string message,Exception reason)
        {
            //restart myself to get new state
            var commandExecutionException = new AggregateActor.CommandExecutionException(message, reason);
            ExecutionContext.CommandSender.Tell(new AggregateActor.CommandFailed(commandExecutionException));
            throw commandExecutionException;
        }


        protected void StashMessage(object message)
        {
            Log.Debug("Stashing message {@message} current behavior is {behavior}", message, Behavior.Current);

            Stash.Stash();
        }

        private void CompleteExecution()
        {
            Log.Info("Command executed. {@context}", ExecutionContext.CommandMetadata);

            if (Aggregate == null)
                throw new InvalidOperationException("Aggregate state was null after command execution");

            if(ExecutionContext.IsWaitingForConfirmation)
                ExecutionContext.CommandSender.Tell(AggregateActor.CommandExecuted.Instance);

            ExecutionContext.Clear();
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
            Stash.Unstash();
        }
    }

    public static class AggregateActor
    {

        public class ExecuteCommand : IHaveMetadata
        {
            public IMessageMetadata Metadata { get; }
            public ICommand Command { get; }
            
            public bool IsWaitingAcknowledgement { get; }

            public ExecuteCommand(ICommand command, IMessageMetadata metadata, bool ack=false)
            {
                Command = command;
                Metadata = metadata;
                IsWaitingAcknowledgement = true;
            }
        }

        public class CommandExecuted
        {
            protected CommandExecuted()
            {
            }

            public static CommandExecuted Instance { get; } = new CommandExecuted();
        }

        public class CommandFailed : CommandExecuted
        {
            public Exception Reason { get; }

            public CommandFailed(Exception reason)
            {
                Reason = reason;
            }
        }

        public class CommandExecutionException : Exception
        {
            public CommandExecutionException(string msg,Exception reason):base(msg, reason)
            {
            }
        }

        public class ShutdownGratefully
        {
            private ShutdownGratefully()
            {
            }

            public static ShutdownGratefully Instance { get; } = new ShutdownGratefully();
        }

        public class CheckHealth
        {
            private CheckHealth()
            {
            }

            public static CheckHealth Instance { get; } = new CheckHealth();
        }
    }
}