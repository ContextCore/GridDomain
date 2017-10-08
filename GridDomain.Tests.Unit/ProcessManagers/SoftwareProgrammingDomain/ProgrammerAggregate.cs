using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{

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

    
    public class CoffeMachineCreated : DomainEvent
    {
        public int MaxCups { get; }

        public CoffeMachineCreated(Guid id, int maxCups):base(id)
        {
            MaxCups = maxCups;
        }
    }

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