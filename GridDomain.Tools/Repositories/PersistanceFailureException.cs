using System;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories
{
    public class PersistanceFailureException : NullReferenceException
    {
        public JournalEntry Entry { get; }

        public PersistanceFailureException(JournalEntry entry, NullReferenceException exception)
            : base("persistance failure", exception)
        {
            Entry = entry;
        }
    }
}