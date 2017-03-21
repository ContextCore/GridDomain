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

        public ISaga<SoftwareProgrammingSagaState> Create(GotTiredEvent message)
        {
            return Create(new SoftwareProgrammingSagaState(message.SagaId,
                                                          nameof(SoftwareProgrammingSaga.Coding)));
        }

        public ISaga<SoftwareProgrammingSagaState> Create(SoftwareProgrammingSagaState message)
        {
            return new Saga<SoftwareProgrammingSagaState>(new SoftwareProgrammingSaga(), message, _log);
        }

        public ISaga<SoftwareProgrammingSagaState> Create(SleptWellEvent message)
        {
            return Create(new SoftwareProgrammingSagaState(message.SagaId,
                                                          nameof(SoftwareProgrammingSaga.Coding)));
        }
    }
}