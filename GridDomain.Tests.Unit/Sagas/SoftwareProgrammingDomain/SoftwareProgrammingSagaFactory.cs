using System;
using GridDomain.Processes;
using GridDomain.Processes.Creation;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingProcessManagerFactory : IProcessManagerCreator<SoftwareProgrammingState>,
                                                  IProcessManagerCreator<SoftwareProgrammingState, GotTiredEvent>,
                                                  IProcessManagerCreator<SoftwareProgrammingState, SleptWellEvent>
    {
        private readonly ILogger _log;

        public SoftwareProgrammingProcessManagerFactory(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Creates new saga from start message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sagaId">id of creating saga</param>
        /// <returns></returns>
        public IProcessManager<SoftwareProgrammingState> CreateNew(GotTiredEvent message, Guid? sagaId = null)
        {
            return Create(new SoftwareProgrammingState(sagaId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }

        public IProcessManager<SoftwareProgrammingState> Create(SoftwareProgrammingState message)
        {
            return new ProcessManager<SoftwareProgrammingState>(new SoftwareProgrammingProcess(), message, _log);
        }

        /// <summary>
        /// Creates new saga from start message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IProcessManager<SoftwareProgrammingState> CreateNew(SleptWellEvent message, Guid? sagaId = null)
        {
            return Create(new SoftwareProgrammingState(sagaId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }
    }
}