using System;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.Sagas.Messages
{
    public class RedirectToNewSaga :  ISagaTransitCompleted
    {
        public IMessageMetadataEnvelop MessageToRedirect { get; }
        public Guid SagaId { get; }

        public RedirectToNewSaga(Guid sagaId, IMessageMetadataEnvelop messageToRedirect)
        {
            SagaId = sagaId;
            MessageToRedirect = messageToRedirect;
        }
    }
}