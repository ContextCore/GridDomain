using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GridDomain.Aggregates.Tests
{
    public enum Mood
    {
        Good,
        Neutral,
        Bad
    }

    public class Cat : IAggregate
    {
        public string Id { get; private set; }
        public string Name => Id;
        
        public Mood Mood { get; private set; }
        public void Apply(DomainEvent @event)
        {
            switch (@event)
            {
                case Born b:
                    Id = b.Name;
                    Mood = Mood.Bad;
                    break;
                
                case GotHungry h:
                    switch (Mood)
                    {
                        case Mood.Bad: break;
                        case Mood.Good: Mood = Mood.Neutral;
                            break;
                        case Mood.Neutral: Mood = Mood.Bad;
                            break;
                    }
                    break;
                
                case Feeded f:
                    Mood = Mood.Good;
                    break;
            }

            Version++;
        }

        public int Version { get; private set; }
        public Task<IReadOnlyCollection<DomainEvent>> Execute(ICommand command)
        {
            switch (command)
            {
                case FeedCommand f:
                    return Task.FromResult((IReadOnlyCollection<DomainEvent>)new [] {new Feeded(f.AggregateId)});
                case GetNewCatCommand c:
                    return Task.FromResult((IReadOnlyCollection<DomainEvent>) new[] {new Born(c.AggregateId)});
                case FeelHungryCommand c:
                    return Task.FromResult((IReadOnlyCollection<DomainEvent>) new[] {new GotHungry(c.AggregateId)});
                default:
                    throw new UnknownCommandException();
            }
        }


        public class GetNewCatCommand : Command<Cat>
        {
            public GetNewCatCommand(string catName):base(catName)
            {
            }
        }
        
        public class FeelHungryCommand : Command<Cat>
        {
            public FeelHungryCommand(string catName):base(catName)
            {
            }
        }
        public class FeedCommand : Command<Cat>
        {
            public FeedCommand(string catName):base(catName)
            {
            }
        }

        
        public class Born: DomainEvent<Cat>
        {
            public string Name { get; }

            public Born(string name) : base(name)
            {
                Name = name;
            }
        }
        
        public class GotHungry:DomainEvent<Cat>
        {
              public GotHungry(string name) : base(name)
              {
              }
        }
        
        public class Feeded:DomainEvent<Cat>
        {
            public Feeded(string name) : base(name)
            {
            }
        }
    }

    public class UnknownCommandException : Exception
    {
    }


    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        }
    }
}