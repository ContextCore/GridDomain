using System;
using System.Collections.Generic;

namespace GridDomain.Tools.Persistence
{
    /// <summary>
    /// Class for reading \ writing data perrsisted in sql db with wire
    /// Use only in emergency cases by own risk!
    /// For example, when you have differen versions of events with same type persisted
    /// for different instance of one aggregate type.
    /// </summary>
    public class SqlJournalRepository
    {
        //public IReadOnlyCollection<JournalEntry> GetEntries(Predicate<JournalEntry> filter)
        //{
        //    return null;
        //}

        //public void Save(JournalEntry e)
        //{
            
        //}
    }
}