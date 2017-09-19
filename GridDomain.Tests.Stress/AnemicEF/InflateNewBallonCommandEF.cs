using System;
using System.Threading.Tasks;
using GridDomain.Tests.Acceptance.BalloonDomain;

namespace GridDomain.Tests.Stress.AnemicEF {
    class InflateNewBallonCommandEF : IEFCommand
    {
        private readonly Func<BalloonContext> _creator;
        private readonly Guid _guid;
        private readonly string _title;

        public InflateNewBallonCommandEF(Guid id, string title, Func<BalloonContext> creator)
        {
            _title = title;
            _guid = id;
            _creator = creator;
        }

        public async Task Execute()
        {
            using(var ctx = _creator())
            {
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = _guid, Title = _title });
                await ctx.SaveChangesAsync();
            }
        }
    }
}