using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using NEventStore.Persistence.Sql;

namespace GridDomain.Node.MessageDump
{


    public class MessageDumpHandler : IHandler<DomainEvent>,
                                 IHandler<ICommand>,
                                 IHandler<ICommandFault>
    {
        private readonly Func<DebugJournalContext> _contextFactory;

        public MessageDumpHandler(Func<DebugJournalContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private void Handle(object msg, Guid id, long ticks)
        {
            var entry = new DebugJournalEntry
            {
                Manifest = msg.GetType().AssemblyQualifiedName,
                SourceId = id.ToString(),
                Timestamp = ticks,
                Payload = msg.ToPropsString()
            };

            using (var context = _contextFactory())
            {
                try
                {
                    context.Journal.Add(entry);
                    context.SaveChanges();
                }
                catch (UniqueKeyViolationException)
                {
                    context.SosJournal.Add(JournalEntryFault.FromEntry(entry));
                    context.SaveChanges();
                }
            }
        }

        public void Handle(DomainEvent msg)
        {
            Handle(msg, msg.SourceId, msg.CreatedTime.Ticks);
        }

        public void Handle(ICommand msg)
        {
            Handle(msg,msg.Id,msg.Time.Ticks);
        }

        public void Handle(ICommandFault msg)
        {
            Handle(msg, msg.Id, msg.Time.Ticks);
        }
    }
}