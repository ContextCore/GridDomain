using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainSaga
    {
        List<object> MessagesToDispatch { get; }
        IAggregate StateAggregate { get; set; }
    }
}