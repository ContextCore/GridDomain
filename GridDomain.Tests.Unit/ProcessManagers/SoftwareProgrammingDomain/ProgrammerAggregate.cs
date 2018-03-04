using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using Quartz.Util;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
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
            Execute<CreatePersonCommand>(c => new ProgrammerAggregate(c.AggregateId, c.AggregateId));
        }

        public ProgrammerAggregate(string id, string personId) : this(id)
        {
            Emit(new[] {new PersonCreated(id, personId)});
        }

        public string PersonId { get; private set; }
        public int SleepTimes { get; private set; }
    }
}