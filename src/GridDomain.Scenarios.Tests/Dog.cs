using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Tests
{  
    
    public enum Mood
    {
        Good,
        Neutral,
        Bad
    }
    
    public class Dog : IAggregate
    {
        public string Id { get; private set; }
        public string Name => Id;
      
        public Mood Mood { get; private set; }
        public void Apply(IDomainEvent @event)
        {
            switch (@event)
            {
                case Born b:
                    Id = b.Name;
                    Mood = Mood.Bad;
                    break;
                case Tired t:
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
        public Task<IReadOnlyCollection<IDomainEvent>> Execute(ICommand command)
        {
            switch (command)
            {
                case FeedCommand f:
                    return new Feeded(Id, Version).AsCommandResult();
                case GetNewDogCommand c:
                    return new Born(c.Recipient.Id,Version).AsCommandResult();
                case FeelHungryCommand c:
                    return new GotHungry(Id,Version).AsCommandResult();
                case PetCommand c:
                    switch (Mood)
                    {
                        case Mood.Good: return new Tired(Name, Version).AsCommandResult();
                        case Mood.Neutral: return new GotHungry(Name, Version).AsCommandResult();
                        case Mood.Bad: throw new IsUnhappyException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new UnknownCommandException();
            }
        }

        public class IsUnhappyException : Exception
        {
        }


        public class GetNewDogCommand : Command<Dog>
        {
            public GetNewDogCommand(string name):base(name)
            {
            }
        }
        
        public class FeelHungryCommand : Command<Dog>
        {
            public FeelHungryCommand(string name):base(name)
            {
            }
        }
        public class FeedCommand : Command<Dog>
        {
            public FeedCommand(string name):base(name)
            {
            }
        }
        
        public class PetCommand : Command<Dog>
        {
            public PetCommand(string name):base(name)
            {
            }
        }


        
        public class Born: DomainEvent<Dog>
        {
            public string Name { get; }

            public Born(string name, long version) : base(name, version)
            {
                Name = name;
            }
        }
        
        public class GotHungry:DomainEvent<Dog>
        {
            public GotHungry(string name, long version) : base(name, version)
            {
            }
        }
        
        public class Tired:DomainEvent<Dog>
        {
            public Tired(string name, long version) : base(name, version)
            {
            }
        }
        
        public class Feeded:DomainEvent<Dog>
        {
            public Feeded(string name, long version) : base(name, version)
            {
            }
        }
        
       
    }
}
