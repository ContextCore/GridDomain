using System;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.StateSagas
{
    [TestFixture]
    internal class CreateSagaGraph
    {
        [Test]
        public void GetGraph()
        {
            var saga = new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.NewGuid(),SoftwareProgrammingSaga.States.Coding));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }
}