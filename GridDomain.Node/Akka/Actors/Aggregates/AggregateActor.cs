using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;
using Akka.Persistence;
using DotNetty.Common.Utilities;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Node.Akka.AggregatesExtension;
using GridDomain.Node.Akka.Logging;

namespace GridDomain.Node.Akka.Actors.Aggregates
{
    
    public class AggregateHealthReport
    {
        public string Location { get; }
            
        public TimeSpan Uptime { get; }

        public AggregateHealthReport(string location, TimeSpan uptime)
        {
            Location = location;
            Uptime = uptime;
        }
    }
    /// <summary>
    ///     Name should be parse by AggregateActorName
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class AggregateActor<TAggregate> : ReceivePersistentActor where TAggregate : class, IAggregate
    {
        protected override ILoggingAdapter Log { get; } = Context.GetSeriLogger();
        public override string PersistenceId { get; }
        protected string Id { get; }
        public TAggregate Aggregate { get; private set; }
        private AggregateCommandExecutionContext ExecutionContext { get; } = new AggregateCommandExecutionContext();
        protected readonly BehaviorQueue Behavior;
        private readonly DateTime _startedTime;

        public AggregateActor() 
        {
            Behavior = new BehaviorQueue(Become);
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
            PersistenceId = Self.Path.Name;
            Id = EntityActorName.Parse<TAggregate>(Self.Path.Name).Id;
            
            var aggregateExtensions = Context.System.GetAggregatesExtension();
            var dependencies = aggregateExtensions.GetDependencies<TAggregate>();
            Aggregate = dependencies.AggregateFactory.Build();
            
            Context.SetReceiveTimeout(dependencies.Configuration.MaxInactivityPeriod);
            Recover<DomainEvent>(e => Aggregate.Apply(e));

            _startedTime = BusinessDateTime.UtcNow;
        }

        protected virtual void AwaitingCommandBehavior()
        {
            Command<ReceiveTimeout>(t =>
            {
                Context.Parent.Tell(new Passivate(AggregateActor.ShutdownGratefully.Instance));
            });
            Command<AggregateActor.ShutdownGratefully>(s => { Context.Stop(Self); });
            Command<AggregateActor.CheckHealth>(c => Sender.Tell(new AggregateHealthReport(Self.Path.ToString(), BusinessDateTime.UtcNow - _startedTime)));
            Command<AggregateActor.ExecuteCommand>(m =>
                                             {
                                                 var cmd = m.Command;
                                                 Log.Debug("Received command {cmdId}",cmd.Id);

                                                 ExecutionContext.Command = cmd;
                                                 ExecutionContext.CommandMetadata = m.Metadata;
                                                 ExecutionContext.CommandSender = Sender;

                                                 try
                                                 {
                                                     Aggregate.Execute(ExecutionContext.Command)
                                                              .PipeTo(Self);
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     StopOnException(ex);
                                                 }
                                                 

                                                 Behavior.Become(ProcessingCommandBehavior, nameof(ProcessingCommandBehavior));
                                             });
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

                                    int messagesToPersistCount = domainEvents.Count;

                                    PersistAll(domainEvents,
                                               persistedEvent =>
                                               {
                                                   Aggregate.ApplyByVersion(persistedEvent);
                                                   
                                                   if (--messagesToPersistCount != 0) return;
                                                   CompleteExecution();
                                               });
                                });

     
            //aggregate raised an error during command execution
            Command<Status.Failure>(f => { StopOnException(f.Cause); });

            CommandAny(StashMessage);
        }

        private void StopOnException(Exception reason)
        {
            ExecutionContext.CommandSender.Tell(new AggregateActor.CommandFailed(reason));
            //restart myself to get new state
            throw new AggregateActor.CommandExecutionException();
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

            ExecutionContext.CommandSender.Tell(AggregateActor.CommandExecuted.Instance);

            ExecutionContext.Clear();
            Behavior.Become(AwaitingCommandBehavior, nameof(AwaitingCommandBehavior));
            Stash.Unstash();
        }
    }

    public static class AggregateActor
    {
        public class ExecuteCommand: IHaveMetadata
        {
            public IMessageMetadata Metadata { get; }
            public ICommand Command { get; }

            public ExecuteCommand(ICommand command, IMessageMetadata metadata)
            {
                Command = command;
                Metadata = metadata;
            }
        }
        public class CommandExecuted
        {
            protected CommandExecuted() { }
            public static CommandExecuted Instance { get; } = new CommandExecuted();
        }
        
        public class CommandFailed:CommandExecuted
        {
            public Exception Reason { get; }
            public CommandFailed(Exception reason)
            {
                Reason = reason;
            }
        }
        
        public class CommandExecutionException : Exception
        {
        }

        public class ShutdownGratefully
        {
            private ShutdownGratefully(){}
            public static ShutdownGratefully Instance { get; } = new ShutdownGratefully();
        }
        
        public class CheckHealth
        {
            private CheckHealth(){}
            public static CheckHealth Instance { get; } = new CheckHealth();
        }

      
    }
}