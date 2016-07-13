using System.Threading;
using Akka.Actor;
using GridDomain.Node;
using Quartz;

namespace GridDomain.Tests.SynchroniousCommandExecute
{

    public class InMemorySampleDomainTests : SampleDomainCommandExecutionTests
    {
        public InMemorySampleDomainTests() : base(true)
        {
        }
    }
}
