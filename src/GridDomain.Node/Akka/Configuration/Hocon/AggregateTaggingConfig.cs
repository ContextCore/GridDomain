using GridDomain.Aggregates;

namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public class AggregateTaggingConfig : IHoconConfig
    {
        private readonly string _journalId;
        public AggregateTaggingConfig(string journalId)
        {
            _journalId = journalId;
        }

        public string Build()
        {
            return
                @"akka.persistence.journal {
            " + _journalId + @"{
                event-adapters {
                    tagging = """ + typeof(AggregateTaggingAdapter).AssemblyQualifiedShortName() + @"""
                }

                event-adapter-bindings {
                    """ + typeof(IDomainEvent).AssemblyQualifiedShortName() + @""" = tagging
                }
            }
        }";
        }
    }
}