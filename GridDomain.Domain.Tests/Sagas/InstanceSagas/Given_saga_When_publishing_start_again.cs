using System;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_again : InMemorySampleDomainTests
    {
        [Then]
        public void Saga_should_work_as_loop()
        {
            throw new NotImplementedException();
        }
    }
}