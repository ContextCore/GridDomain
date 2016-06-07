using System;
using NMoneys;

namespace BusinessNews.Domain.OfferAggregate
{
    //for demo purposes offers is predefined 
    public abstract class Offer
    {
        public Guid Id { get; protected set; }
        public Money Price { get; protected set; }
        public string[] Grants { get; protected set; }
        public string Name { get; protected set; }
        public TimeSpan Period { get; protected set; }
    }
}