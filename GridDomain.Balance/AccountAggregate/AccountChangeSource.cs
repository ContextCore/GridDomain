using System;

namespace BusinessNews.Domain.AccountAggregate
{
    public class AccountChangeSource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
    }
}