using System;
using System.Threading.Tasks;
using GridDomain.Tests.Acceptance.BalloonDomain;

namespace GridDomain.Tests.Stress.AnemicEF {
    class UpdateBallonCommandEF : IEFCommand
    {
        private readonly Func<BalloonContext> _creator;
        private readonly string _guid;
        private readonly string _title;

        public UpdateBallonCommandEF(string id, string title, Func<BalloonContext> creator)
        {
            _title = title;
            _guid = id;
            _creator = creator;
        }

        public async Task Execute()
        {
            using(var ctx = _creator())
            {
                var catalog = await ctx.BalloonCatalog.FindAsync(_guid);
                catalog.Title = _title;
                await ctx.SaveChangesAsync();
            }
        }
    }
}