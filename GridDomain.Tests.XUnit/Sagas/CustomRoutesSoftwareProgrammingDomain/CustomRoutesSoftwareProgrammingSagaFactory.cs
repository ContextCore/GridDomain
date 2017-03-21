using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain
{
    public class CustomRoutesSoftwareProgrammingSagaFactory :
        IFactory<ISaga<SoftwareProgrammingSagaState>, SoftwareProgrammingSagaState>,
        IFactory<ISaga<SoftwareProgrammingSagaState>, GotTiredEvent>,
        IFactory<ISaga<SoftwareProgrammingSagaState>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public CustomRoutesSoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<SoftwareProgrammingSagaState> Create(GotTiredEvent message)
        {
            return new Saga<SoftwareProgrammingSagaState>(new CustomRoutesSoftwareProgrammingSaga(),
                                                          new SoftwareProgrammingSagaState(message.PersonId,
                                                                                           new CustomRoutesSoftwareProgrammingSaga().Coding.Name),
                                                          _log);
        }

        public ISaga<SoftwareProgrammingSagaState> Create(SoftwareProgrammingSagaState message)
        {
            return new Saga<SoftwareProgrammingSagaState>(new CustomRoutesSoftwareProgrammingSaga(), message, _log);
        }

        public ISaga<SoftwareProgrammingSagaState> Create(SleptWellEvent message)
        {
            return new Saga<SoftwareProgrammingSagaState>(new CustomRoutesSoftwareProgrammingSaga(),
                                                          new SoftwareProgrammingSagaState(message.SofaId,
                                                                                           new CustomRoutesSoftwareProgrammingSaga().Sleeping.Name),
                                                          _log);
        }
    }
}