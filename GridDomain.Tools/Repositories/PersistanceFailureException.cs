using System;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories
{
    public class PersistanceFailureException : NullReferenceException
    {
        public PersistanceFailureException(JournalItem item, NullReferenceException exception)
            : base("persistance failure", exception)
        {
            Item = item;
        }

        public JournalItem Item { get; }
    }
}