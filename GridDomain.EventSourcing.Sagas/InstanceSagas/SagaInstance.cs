﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Automatonymous;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.Logging;
using Serilog;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class SagaInstance
    {
        public static SagaInstance<TSaga, TSagaData> New<TSaga, TSagaData>(TSaga saga, SagaStateAggregate<TSagaData> data)
            where TSaga : Saga<TSagaData>
            where TSagaData : class, ISagaState
        {
            return new SagaInstance<TSaga, TSagaData>(saga, data);
        }
    }

    public class SagaInstance<TSaga, TSagaData> : ISagaInstance<TSaga, TSagaData>
        where TSaga : Saga<TSagaData>
        where TSagaData : class, ISagaState
    {
        public readonly Saga<TSagaData> Machine;
        private readonly SagaStateAggregate<TSagaData> _dataAggregate;

        private static readonly ILogger Log = Serilog.Log.Logger.ForContext<SagaInstance<TSaga, TSagaData>>();
        
        private List<ICommand> _commandsToDispatch = new List<ICommand>();
        public IReadOnlyCollection<ICommand> CommandsToDispatch => _commandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.ClearCommands();
            _commandsToDispatch.Clear();
        }

        SagaStateAggregate<TSagaData> ISagaInstance<TSaga, TSagaData>.Data => _dataAggregate;

        public IAggregate Data => _dataAggregate;

        public SagaInstance(Saga<TSagaData> machine, 
                            SagaStateAggregate<TSagaData> dataAggregate, 
                            bool doUninitializedWarnings = true)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));
            if (dataAggregate == null) throw new ArgumentNullException(nameof(dataAggregate));
            _dataAggregate = dataAggregate;
            Machine = machine;

            CheckInitialState(dataAggregate, doUninitializedWarnings);
        }

        private string CurrentStateName => _dataAggregate.Data?.CurrentStateName;

        private bool CheckInitialState(SagaStateAggregate<TSagaData> dataAggregate, bool logUninitializedState = true)
        {
            if (!string.IsNullOrEmpty(CurrentStateName)) return true;

            Log.Warning("Started saga {Type} {Id} without initialization.", GetType().Name, dataAggregate.Id);
            Log.Warning(_dataAggregate.Data == null ? "Saga data is empty" : "Current state name is not specified");

            if (!logUninitializedState) return false;

            Log.Warning("Saga will not process and only record incoming messages");
            return false;
        }

        public async Task Transit<TMessage>(TMessage message) where TMessage : class
        {
            //Saga is not initialized
            if (_dataAggregate.Id == Guid.Empty)
            {
                Log.Verbose("Saga {Saga} id is empty and it received message {Message}", typeof(TSaga).Name,message);
                return;
            }

            if (_dataAggregate.Data== null)
            {
                Log.Verbose("Saga {Saga} data is empty and it received message {Message}", typeof(TSaga).Name,message);
                return;
            }

            var machineEvent = Machine.GetMachineEvent(message);

            try
            {
                await Machine.RaiseEvent(_dataAggregate.Data, machineEvent, message);
            }
            catch (Exception ex)
            {
                throw new SagaTransitionException(message, _dataAggregate.Data, ex);
            }

            _dataAggregate.RememberEvent(machineEvent, _dataAggregate.Data, message);
            _commandsToDispatch = Machine.CommandsToDispatch.Select(c => c.CloneWithSaga(Data.Id))
                                                            .Cast<ICommand>()
                                                            .ToList();
        }
    }
}
