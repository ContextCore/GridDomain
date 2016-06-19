using System;
using System.Linq;
using System.Threading;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Akka.Messages;
using NLog;
using Stateless;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSaga : StateSaga<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        public enum States
        {
            Created,
            MessageSent,
            SuccessfullyProcessed,
            ProcessingFailure
        }

        public enum Transitions
        {
            SendMessage,
            Success,
            Failure
        }

        public ScheduledCommandProcessingSaga(ScheduledCommandProcessingSagaState state) : base(state)
        {
            Machine.Configure(States.Created)
                .Permit(Transitions.SendMessage, States.MessageSent);

            //StateMachine<States, Transitions>.TriggerWithParameters<CommandFault<Command>> commandFailure = RegisterCommandFault<Command>(Transitions.Failure);
            var messageSent = RegisterEvent<ScheduledCommandProcessingStarted>(Transitions.SendMessage);
            var successTransition = RegisterEvent<ScheduledCommandSuccessfullyProcessed>(Transitions.Success);
            //var failTransition = RegisterEvent<ScheduledCommandProcessingFailed>(Transitions.Failure);
            Machine
                .Configure(States.MessageSent)
                .OnEntryFrom(messageSent, e =>
                {
                    var triggerWithParameters = RegisterGenericCommandFault(e.Command, Transitions.Failure);
                    //Machine.Configure(States.ProcessingFailure).OnEntryFrom(triggerWithParameters, @event =>
                    //{
                    //    _log.Error(@event.Exception, $"saga failure {State.Key.Name}");
                    //});


                    var configuration = Machine.Configure(States.ProcessingFailure);
                    var commandFaultType = typeof(CommandFault<>).MakeGenericType(e.Command.GetType());
                    var triggers = typeof(StateMachine<,>.TriggerWithParameters<>).MakeGenericType(typeof(States), typeof(Transitions), commandFaultType);
                    var action = typeof(Action<>).MakeGenericType(commandFaultType);
                    var onEntryFrom = configuration.GetType().GetMethods().FirstOrDefault(x => x.Name == nameof(configuration.OnEntryFrom) && x.IsGenericMethod && x.GetGenericArguments().Length == 1).MakeGenericMethod(commandFaultType);
                    Action<dynamic> a = @event => { _log.Error(@event.Exception); };
                    onEntryFrom.Invoke(configuration, new object[] { triggerWithParameters, a, null });
                    Dispatch(e.Command);
                })
                .Permit(Transitions.Success, States.SuccessfullyProcessed)
                .Permit(Transitions.Failure, States.ProcessingFailure);
            //Machine.Configure(States.ProcessingFailure).OnEntryFrom(commandFailure, @event => { });
            Machine.Configure(States.SuccessfullyProcessed).OnEntryFrom(successTransition, @event =>
             {
                 _log.Error($"saga processed {State.Key.Name}");
             });
            //Machine.Configure(States.ProcessingFailure).OnEntryFrom(failTransition, @event =>
            // {
            //     _log.Error($"saga processing fail {State.Key.Name}");
            // });


        }

        private object RegisterGenericCommandFault(Command command, Transitions transition)
        {
            //StateMachine<States, Transitions>.TriggerWithParameters<CommandFault<Command>>
            var method = typeof(StateSaga<States, Transitions, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>).GetMethods().FirstOrDefault(x => x.Name == nameof(RegisterCommandFault));
            var generic = method.MakeGenericMethod(command.GetType());
            return generic.Invoke(this, new object[] { transition });
        }

        public static ISagaDescriptor SagaDescriptor => new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
    }
}