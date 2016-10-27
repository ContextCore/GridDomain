using System;
using GridDomain.Tools.Persistence.SqlPersistence;

namespace GridDomain.Tools.Repositories
{
    public class PersistanceFailureException : NullReferenceException
    {
        public JournalItem Item { get; }

        public PersistanceFailureException(JournalItem item, NullReferenceException exception)
            : base("persistance failure", exception)
        {
            Item = item;
        }
    }
}