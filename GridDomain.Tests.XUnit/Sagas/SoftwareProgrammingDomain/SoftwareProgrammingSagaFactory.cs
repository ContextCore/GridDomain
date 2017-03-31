using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaFactory :
        IFactory<ISaga<SoftwareProgrammingSagaState>, SoftwareProgrammingSagaState>,
        IFactory<ISaga<SoftwareProgrammingSagaState>, GotTiredEvent>,
        IFactory<ISaga<SoftwareProgrammingSagaState>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public SoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }
        /// <summary>
        /// Creates new saga from start message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ISaga<SoftwareProgrammingSagaState> Create(GotTiredEvent message)
        {
            var sagaId = message.SagaId == Guid.Empty ? Guid.NewGuid() : message.SagaId;
            return Create(new SoftwareProgrammingSagaState(sagaId,
                                                           nameof(SoftwareProgrammingSaga.Coding)));
        }

        public ISaga<SoftwareProgrammingSagaState> Create(SoftwareProgrammingSagaState message)
        {
            return new Saga<SoftwareProgrammingSagaState>(new SoftwareProgrammingSaga(), message, _log);
        }

        /// <summary>
        /// Creates new saga from start message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ISaga<SoftwareProgrammingSagaState> Create(SleptWellEvent message)
        {
            var sagaId = message.SagaId == Guid.Empty ? Guid.NewGuid() : message.SagaId;
            return Create(new SoftwareProgrammingSagaState(sagaId,
                                                           nameof(SoftwareProgrammingSaga.Coding)));
        }
    }
}