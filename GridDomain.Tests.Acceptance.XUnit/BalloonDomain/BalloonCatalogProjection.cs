using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    class BalloonCatalogProjection : IHandler<BalloonTitleChanged>,
                                     IHandler<BalloonCreated>

    {
        private readonly Func<BallonContext> _contextCreator;

        public BalloonCatalogProjection(Func<BallonContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            using (var context = _contextCreator())
            {
                context.BalloonCatalog.AddOrUpdate(msg.ToCatalogItem());
                return context.SaveChangesAsync();
            }
        }

        public Task Handle(BalloonCreated msg)
        {
            using (var context = _contextCreator())
            {
                context.BalloonCatalog.Add(msg.ToCatalogItem());
                return context.SaveChangesAsync();
            }
        }
    }
}
