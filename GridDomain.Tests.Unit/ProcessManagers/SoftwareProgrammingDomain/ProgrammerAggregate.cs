using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class ProgrammerAggregate : ConventionAggregate
    {
        private ProgrammerAggregate(Guid id) : base(id)
        {
            Apply<PersonCreated>(e =>
                                 {
                                     Id = e.Id;
                                     PersonId = e.PersonId;
                                 });
            Apply<Slept>(e => SleepTimes++);
            Execute<GoSleepCommand>(async c =>
                                    {
                                        Guid sofaId = c.SofaId;
                                        if(sofaId == Guid.Empty)
                                            throw new CantFindSofaException();

                                        await Task.Delay(TimeSpan.FromMilliseconds(5));
                                        await Emit(new Slept(sofaId));
                                    });
            Execute<CreatePersonCommand>(c => new ProgrammerAggregate(c.AggregateId, c.AggregateId));
        }

        public ProgrammerAggregate(Guid id, Guid personId) : this(id)
        {
            Produce(new PersonCreated(id, personId));
        }

        public Guid PersonId { get; private set; }
        public int SleepTimes { get; private set; }
    }
}