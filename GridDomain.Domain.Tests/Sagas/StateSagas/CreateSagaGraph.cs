using System;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    internal class CreateSagaGraph
    {
        [Test]
        public void GetGraph()
        {
            var saga = new SoftwareProgrammingSaga(new SoftwareProgrammingSagaState(Guid.NewGuid(),SoftwareProgrammingSaga.States.Working));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }
}