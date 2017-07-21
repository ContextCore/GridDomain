using System;
using GridDomain.Processes;
using GridDomain.Processes.Creation;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
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
        /// Creates new process from start message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="processId">id of creating process</param>
        /// <returns></returns>
        public IProcessManager<SoftwareProgrammingState> CreateNew(GotTiredEvent message, Guid? processId = null)
        {
            return Create(new SoftwareProgrammingState(processId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }

        public IProcessManager<SoftwareProgrammingState> Create(SoftwareProgrammingState message)
        {
            return new ProcessManager<SoftwareProgrammingState>(new SoftwareProgrammingProcess(), message, _log);
        }

        /// <summary>
        /// Creates new process from start message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IProcessManager<SoftwareProgrammingState> CreateNew(SleptWellEvent message, Guid? processId = null)
        {
            return Create(new SoftwareProgrammingState(processId ?? Guid.NewGuid(),
                                                           nameof(SoftwareProgrammingProcess.Coding)));
        }
    }
}