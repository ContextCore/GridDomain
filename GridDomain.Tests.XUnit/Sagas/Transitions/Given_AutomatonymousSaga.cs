using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga
    {
        public readonly SagaStateAggregate<SoftwareProgrammingSagaData> SagaDataAggregate;
        public readonly Saga<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> SagaInstance;
        public readonly SoftwareProgrammingSaga SagaMachine = new SoftwareProgrammingSaga();

        public Given_AutomatonymousSaga(Func<SoftwareProgrammingSaga, State> initialState, ILogger logger)
        {
            var sagaData = new SoftwareProgrammingSagaData(Guid.NewGuid(), initialState(SagaMachine).Name);
            SagaDataAggregate = new SagaStateAggregate<SoftwareProgrammingSagaData>(sagaData);
            SagaInstance = new Saga<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>(SagaMachine,
                                                                                          SagaDataAggregate.Data,
                                                                                          logger);
        }
    }
}