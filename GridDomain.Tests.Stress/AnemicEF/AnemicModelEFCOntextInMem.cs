using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Stress.NodeCommandExecution;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AnemicEF {
    public class AnemicModelEFCOntextInMem : AnemicModelEFContextPerformance
    {
        public AnemicModelEFCOntextInMem(ITestOutputHelper output):
            base(output, new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(CommandExecutionWithProjection)).Options)
        {
            
        }
    }
}