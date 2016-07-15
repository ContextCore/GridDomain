using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    //class SoftwareProgrammingSagaFactory : 
    //    ISagaFactory<SoftwareProgrammingSaga,, SoftwareProgrammingSagaState>,
    //    ISagaFactory<SoftwareProgrammingSaga, GotTiredDomainEvent>,
    //    IEmptySagaFactory<SoftwareProgrammingSaga>
    //{
    //    public SoftwareProgrammingSaga Create(SoftwareProgrammingSagaState message)
    //    {
    //        return new SoftwareProgrammingSaga(message);
    //    }

    //    public SoftwareProgrammingSaga Create(GotTiredEvent message)
    //    {
    //        return new SoftwareProgrammingSaga();
    //    }

    //    public SoftwareProgrammingSaga Create()
    //    {
    //        return new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.Empty,StateSagas.SampleSaga.SoftwareProgrammingSaga.States.Working));
    //    }
    //}
}