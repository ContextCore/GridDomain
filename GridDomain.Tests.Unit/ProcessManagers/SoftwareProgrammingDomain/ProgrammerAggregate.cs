using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class ProgrammerAggregate : CommandAggregate
    {
        private ProgrammerAggregate(Guid id) : base(id)
        {
            Apply<HomeCreated>(e => PersonId = e.PersonId);
            Apply<Slept>(e => SleepTimes++);
            Execute<GoSleepCommand>(c =>
                                    {
                                        Guid sofaId = c.SofaId;
                                        if(sofaId == Guid.Empty)
                                            throw new CantFindSofaException();

                                        Produce(new Slept(sofaId));
                                    });
            Execute<CreatePersonCommand>(c => new ProgrammerAggregate(c.AggregateId, c.AggregateId));
        }

        public ProgrammerAggregate(Guid id, Guid personId) : this(id)
        {
            Produce(new HomeCreated(id, personId));
        }

        public Guid PersonId { get; private set; }
        public int SleepTimes { get; private set; }
    }
}