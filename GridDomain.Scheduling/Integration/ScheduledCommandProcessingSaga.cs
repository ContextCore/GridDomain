using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Akka.Messages;
using NLog;
using Stateless;
using ICommand = System.Windows.Input.ICommand;

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

        public override void Transit(object msg)
        {
           var fault = msg as ICommandFault;//;
            if (fault != null)
            {
                TransitState(fault);
                return;
            }

            base.Transit(msg);
        }

        public ScheduledCommandProcessingSaga(ScheduledCommandProcessingSagaState state) : base(state)
        {
            Machine.Configure(States.Created)
                .Permit(Transitions.SendMessage, States.MessageSent);

            
            var messageSent = RegisterEvent<ScheduledCommandProcessingStarted>(Transitions.SendMessage);
            var successTransition = RegisterEvent<ScheduledCommandSuccessfullyProcessed>(Transitions.Success);

            var trigger = RegisterEvent<ICommandFault>(Transitions.Failure);
            Machine.Configure(States.ProcessingFailure)
                 .OnEntryFrom(trigger, fault =>
                 {
                     _log.Error(fault.Exception);
                 });

            Machine
                .Configure(States.MessageSent)
                .OnEntryFrom(messageSent, e =>
                {

                    Dispatch(e.Command);
                })
                .Permit(Transitions.Success, States.SuccessfullyProcessed);
        }
        public static ISagaDescriptor SagaDescriptor => new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
    }
}