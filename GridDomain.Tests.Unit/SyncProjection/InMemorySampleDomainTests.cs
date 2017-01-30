using GridDomain.Tests.Unit.Sagas.InstanceSagas;

namespace GridDomain.Tests.Unit.SyncProjection
{
    public class InMemorySampleDomainTests : SampleDomainCommandExecutionTests
    {
        public InMemorySampleDomainTests() : base(true)
        {
        }
    }
}