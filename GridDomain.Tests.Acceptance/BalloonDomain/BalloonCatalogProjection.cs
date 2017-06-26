using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.BalloonDomain.Events;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;
using Serilog;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    class BalloonCatalogProjection : IHandlerWithMetadata<BalloonTitleChanged>,
                                     IHandlerWithMetadata<BalloonCreated>

    {
        private readonly Func<BalloonContext> _contextCreator;
        private readonly IPublisher _publisher;
        private readonly ProcessEntry _readModelUpdatedProcessEntry = new ProcessEntry(nameof(BalloonCatalogProjection), "Publishing notification", "Read model was updated");
        private readonly ILogger _log;

        public BalloonCatalogProjection(Func<BalloonContext> contextCreator, IPublisher publisher, ILogger logger)
        {
            _log = logger;
            _publisher = publisher;
            _contextCreator = contextCreator;
        }

        public Task Handle(BalloonTitleChanged msg, IMessageMetadata metadata=null)
        {
            _log.Debug("Projecting balloon catalog from message {@msg}", msg);
            using (var context = _contextCreator())
            {
                context.BalloonCatalog.AddOrUpdate(msg.ToCatalogItem());
                context.SaveChanges();

                _publisher.Publish(new BalloonTitleChangedNotification(){BallonId = msg.SourceId},
                                    metadata.CreateChild(Guid.NewGuid(), _readModelUpdatedProcessEntry));
            }
            _log.Debug("Projected balloon catalog from message {@msg}", msg);

            return Task.CompletedTask;
        }

        public Task Handle(BalloonCreated msg, IMessageMetadata metadata=null)
        {
            _log.Debug("Projecting balloon catalog from message {@msg}", msg);
            using (var context = _contextCreator())
            {
                context.BalloonCatalog.Add(msg.ToCatalogItem());
                context.SaveChanges();
                _publisher.Publish(new BalloonCreatedNotification() { BallonId = msg.SourceId },
                                   metadata.CreateChild(Guid.NewGuid(), _readModelUpdatedProcessEntry));
            }
            _log.Debug("Projected balloon catalog from message {@msg}", msg);
            return Task.CompletedTask;
        }

        public Task Handle(BalloonTitleChanged msg)
        {
            return Handle(msg, null);
        }

        public Task Handle(BalloonCreated msg)
        {
            return Handle(msg, null);
        }
    }
}
