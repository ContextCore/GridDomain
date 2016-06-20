using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonDomain;
using GridDomain.CQRS;
using Stateless;

namespace GridDomain.EventSourcing.Sagas
{
  

    public class StateSaga<TSagaStates, TSagaTriggers, TStateData, TStartMessage> :
                                                            IDomainStateSaga<TStateData>,
                                                            ISagaDescriptor
                                                            where TSagaTriggers : struct
                                                            where TSagaStates : struct
                                                            where TStateData : SagaStateAggregate<TSagaStates, TSagaTriggers>
                                                            where TStartMessage : DomainEvent
    {
        private readonly IDictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>
            _eventsToTriggersMapping
                = new Dictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>();

        public IReadOnlyCollection<Type> AcceptMessages => _eventsToTriggersMapping.Keys.ToArray();
        public IReadOnlyCollection<Type> ProduceCommands => _registeredCommands;

        public Type StartMessage { get; } = typeof (TStartMessage);
        public Type StateType { get; } = typeof (TStateData);
        public Type SagaType => this.GetType();

        public readonly StateMachine<TSagaStates, TSagaTriggers> Machine;

        public TStateData State { get; }

        public StateMachine<TSagaStates, TSagaTriggers>
            .TriggerWithParameters<CommandFault<TCommand>> RegisterCommandFault<TCommand>(TSagaTriggers trigger) where TCommand : ICommand
        {
            _registeredCommands.Add(typeof(TCommand));
            return RegisterEvent<CommandFault<TCommand>>(trigger);
        }


        //public StateMachine<TSagaStates, TSagaTriggers>
        //    .TriggerWithParameters<ICommandFault> RegisterAnyCommandFault(TSagaTriggers trigger): ICommand
        //{
        //    return RegisterEvent<CommandFault<TCommand>>(trigger);
        //}

        private readonly List<Type> _registeredCommands = new List<Type>(); 

        protected StateSaga(TStateData state)
        {
            State = state;

            //to include start message into list of accept messages
            _eventsToTriggersMapping[typeof (TStartMessage)] = null; 
            Machine = new StateMachine<TSagaStates, TSagaTriggers>(State.MachineState);
            Machine.OnTransitioned(t => State.StateChanged(t.Trigger, t.Destination));
            _transitMethod = GetType().GetMethod(nameof(TransitState),BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public TSagaStates DomainState => State.MachineState;
        private readonly List<object> _messagesToDispatch = new List<object>();

        public IReadOnlyCollection<object> CommandsToDispatch => _messagesToDispatch;

        public void ClearCommandsToDispatch()
        {
            _messagesToDispatch.Clear();
        }

        IAggregate IDomainSaga.State => State;
 
        private readonly MethodInfo _transitMethod;
        public virtual void Transit(object msg)
        {
            MethodInfo genericTransit = _transitMethod.MakeGenericMethod(msg.GetType());
            genericTransit.Invoke(this, new [] { msg});
        }

        protected void Dispatch(Command command)
        {
            command.SagaId = State.Id;
            _messagesToDispatch.Add(command);
        }

        protected void DispatchSagaFault<T>(CommandFault<T> commandFault) where T:ICommand
        {
            _messagesToDispatch.Add(new SagaFault<TStateData>(this, commandFault));
        }
        protected void Dispatch(ISagaFault sagaFault)
        {
            _messagesToDispatch.Add(sagaFault);
        }

        protected StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<TEvent> RegisterEvent<TEvent>(
                                                                                                    TSagaTriggers trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<TEvent>(trigger);
            _eventsToTriggersMapping[typeof(TEvent)] = triggerWithParameters;
            return triggerWithParameters;
        }

        protected StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters RegisterMessage(Type messageType, TSagaTriggers trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<object>(trigger);
            _eventsToTriggersMapping[messageType] = triggerWithParameters;
            return triggerWithParameters;
        }

        //TODO: make method non-generic
        protected internal void TransitState<T>(T message)
        {
            StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters triggerWithParameters;
            if (!_eventsToTriggersMapping.TryGetValue(typeof(T), out triggerWithParameters))
                throw new UnbindedMessageRecievedException(message);

            if (Machine.CanFire(triggerWithParameters.Trigger))
            {
                var withParameters = (StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<T>) triggerWithParameters;
                Machine.Fire(withParameters, message);
            }
        }
    }

    //TODO: add policy for command unexpected failure handling
    public class StateSaga<TState, TTrigger, TStartMessage> :
        StateSaga<TState, TTrigger, SagaStateAggregate<TState, TTrigger>, TStartMessage>
        where TTrigger : struct
        where TState : struct
        where TStartMessage : DomainEvent
    {
        public StateSaga(SagaStateAggregate<TState, TTrigger> state) : base(state)
        {
        }
    }
}