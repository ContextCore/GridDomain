using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaFactory : ISagaCreator<SoftwareProgrammingState>,
                                                  ISagaCreator<SoftwareProgrammingState, GotTiredEvent>,
                                                  ISagaCreator<SoftwareProgrammingState, SleptWellEvent>
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
        /// <param name="sagaId">id of creating saga</param>
        /// <returns></returns>
        public ISaga<SoftwareProgrammingState> CreateNew(GotTiredEvent message, Guid? sagaId = null)
        {
            return Create(new SoftwareProgrammingState(sagaId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }

        public ISaga<SoftwareProgrammingState> Create(SoftwareProgrammingState message)
        {
            return new Saga<SoftwareProgrammingState>(new SoftwareProgrammingProcess(), message, _log);
        }

        /// <summary>
        /// Creates new saga from start message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ISaga<SoftwareProgrammingState> CreateNew(SleptWellEvent message, Guid? sagaId = null)
        {
            return Create(new SoftwareProgrammingState(sagaId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }
    }
}