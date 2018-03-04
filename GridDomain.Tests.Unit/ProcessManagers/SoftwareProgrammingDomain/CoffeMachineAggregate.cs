using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain {
    public class CoffeMachineAggregate : ConventionAggregate
    {
        public CoffeMachineAggregate(string id, int maxCups):this(id)
        {
            Emit(new[] {new CoffeMachineCreated(id,maxCups)});
        }

        public int CupsLeft;
        public int NotMadeCups;
        private CoffeMachineAggregate(string id) : base(id)
        {
            Apply<CoffeMakeFailedEvent>(e => NotMadeCups--);
            Apply<CoffeMakeFailedEvent>(e => CupsLeft--);
            Apply<CoffeMachineCreated>(e => CupsLeft = e.MaxCups);

            Execute<MakeCoffeCommand>(c =>
                                      {
                                          if (CupsLeft <= 0)
                                          {
                                              Emit(new[] {new CoffeMakeFailedEvent(Id, c.PersonId)});
                                          }
                                          else
                                          {
                                              Emit(new[] {new CoffeMadeEvent(Id, c.PersonId)});
                                          }
                                      });
        }
    }
}