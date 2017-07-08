using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Scheduling.Akka
{
    /// <summary>
    /// Listening to scheduling events from aggregates and modify quartz jobs accordinally
    /// </summary>
    class FutureEventsShedulingMessageHandler : IHandler<FutureEventScheduledEvent>,
                                                IHandler<FutureEventCanceledEvent>
    {
        public FutureEventsShedulingMessageHandler()
        {
           //switch(evt)
           //{
           //    case FutureEventScheduledEvent e:
           //        Handle(e, commandMetadata);
           //        break;
           //    case FutureEventCanceledEvent e:
           //        Handle(e, commandMetadata);
           //        break;
           //}

        }

        public Task Handle(FutureEventScheduledEvent message, IMessageMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public Task Handle(FutureEventCanceledEvent message, IMessageMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
