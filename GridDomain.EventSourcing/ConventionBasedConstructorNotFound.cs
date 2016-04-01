using System;

namespace GridDomain.EventSourcing
{
    public class ConventionBasedConstructorNotFound : Exception
    {
        public ConventionBasedConstructorNotFound():base("Не найден приватный контсруктор с единственным параметров Guid id, необходимый для " +
                                                         "создания агрегата через EventStore ")
        {
            
        }
    }
}