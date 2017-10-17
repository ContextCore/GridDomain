using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain {
    public class CoffeMachineAggregate : ConventionAggregate
    {
        public CoffeMachineAggregate(Guid id, int maxCups):this(id)
        {
            Produce(new CoffeMachineCreated(id,maxCups));
        }

        public int CupsLeft;
        public int NotMadeCups;
        private CoffeMachineAggregate(Guid id) : base(id)
        {
            Apply<CoffeMakeFailedEvent>(e => NotMadeCups--);
            Apply<CoffeMakeFailedEvent>(e => CupsLeft--);
            Apply<CoffeMachineCreated>(e => CupsLeft = e.MaxCups);

            Execute<MakeCoffeCommand>(c =>
                                      {
                                          if (CupsLeft <= 0)
                                              Produce(new CoffeMakeFailedEvent(Id, c.PersonId));
                                          else
                                              Produce(new CoffeMadeEvent(Id, c.PersonId));
                                      });
        }
    }
}