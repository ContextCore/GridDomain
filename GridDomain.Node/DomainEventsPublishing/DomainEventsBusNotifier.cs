using System;
using System.Linq;
using GridDomain.CQRS.Messaging;
using NEventStore;
using NLog;

namespace GridDomain.Node.DomainEventsPublishing
{
    
    public class DomainEventsBusNotifier : IObserver<ICommit>
    {
        private readonly IPublisher _bus;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DomainEventsBusNotifier(IPublisher bus)
        {
            _bus = bus;
        }

        public void OnNext(ICommit commit)
        {
            //TODO: сделать простой viewer для коммитов в event store
            _log.Trace($"Получен коммит {commit.CommitId} из корзины {commit.BucketId}");

            //TODO: понять,почему попадаются null - события
            foreach (var e in commit.Events.Where(e => e!=null))
            {
               _log.Trace($"Отправляем в шину событие {e.Body}");
               _bus.Publish(e.Body);
            }
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnCompleted()
        {
        
        }
    }
}