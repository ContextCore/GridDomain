using System;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class CustomEvent
    {
        public Guid SagaId { get; set; }
        public string Payload { get; set; }
    }
}