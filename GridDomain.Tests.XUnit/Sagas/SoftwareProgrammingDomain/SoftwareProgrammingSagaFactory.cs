using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaFactory :
        ISagaFactory
            <ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>,
            SagaStateAggregate<SoftwareProgrammingSagaData>>,
        ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent>,
        ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SleptWellEvent>
    {
        private readonly ILogger _log;

        public SoftwareProgrammingSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SagaId,
                    nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(
            SagaStateAggregate<SoftwareProgrammingSagaData> message)
        {
            return SagaInstance.New(new SoftwareProgrammingSaga(), message, _log);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SleptWellEvent message)
        {
            var data =
                new SagaStateAggregate<SoftwareProgrammingSagaData>(new SoftwareProgrammingSagaData(message.SagaId,
                    nameof(SoftwareProgrammingSaga.Coding)));
            return Create(data);
        }
    }
}