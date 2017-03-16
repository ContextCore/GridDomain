using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaFactory :
        ISagaFactory
        <ISaga<SoftwareProgrammingSagaData>,
            SagaStateAggregate<SoftwareProgrammingSagaData>>,
        ISagaFactory<ISaga<SoftwareProgrammingSagaData>, GotTiredEvent>,
        ISagaFactory<ISaga<SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public SoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SagaId,
                                                                                                    nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }

        public ISaga<SoftwareProgrammingSagaData> Create(
            SagaStateAggregate<SoftwareProgrammingSagaData> message)
        {
            return Saga.New(new SoftwareProgrammingSaga(), message, _log);
        }

        public ISaga<SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SagaId,
                                                                                                    nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }
    }
}