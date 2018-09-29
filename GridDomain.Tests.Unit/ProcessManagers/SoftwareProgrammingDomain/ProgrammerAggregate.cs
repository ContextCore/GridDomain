using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{

    public class ProgrammerAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new AggregateDependencies<ProgrammerAggregate>());
        }
    }
    
    public class ProgrammerAggregate : ConventionAggregate
    {
        private ProgrammerAggregate(string id) : base(id)
        {
            Apply<PersonCreated>(e =>
                                 {
                                     Id = e.SourceId;
                                     PersonId = e.PersonId;
                                 });
            Apply<Slept>(e => SleepTimes++);
            Execute<GoSleepCommand>(async c =>
                                    {
                                        var sofaId = c.SofaId;
                                        if(string.IsNullOrEmpty(sofaId))
                                            throw new CantFindSofaException();

                                        await Task.Delay(TimeSpan.FromMilliseconds(5));
                                        Emit(new Slept(sofaId));
                                    });
            Execute<CreatePersonCommand>(c => Emit(new PersonCreated(c.AggregateId, c.AggregateId)));
        }

        public ProgrammerAggregate(string id, string personId) : this(id)
        {
            Emit(new PersonCreated(id, personId));
        }

        public string PersonId { get; private set; }
        public int SleepTimes { get; private set; }
    }
}