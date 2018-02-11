using System;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Common;
using Microsoft.EntityFrameworkCore;
using NBench;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AnemicEF {

    public class AnemicModelEFContextPerformanceSql : AnemicModelEFContextPerformance
    {
        private static readonly string ReadDbReadModelConnectionString = new AutoTestLocalDbConfiguration().ReadModelConnectionString;
        private static readonly DbContextOptions<BalloonContext> DbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(ReadDbReadModelConnectionString).Options;

        public AnemicModelEFContextPerformanceSql(ITestOutputHelper output):base(output, DbContextOptions)
        
        {
          
        }

        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            //warm up EF 
            using(var ctx = new BalloonContext(DbContextOptions))
            {
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = Guid.NewGuid().ToString(), LastChanged = DateTime.UtcNow, Title = "WarmUp" });
                ctx.SaveChanges();
            }

            TestDbTools.Truncate(ReadDbReadModelConnectionString,"BalloonCatalog")
                       .Wait();
        }
    }
}