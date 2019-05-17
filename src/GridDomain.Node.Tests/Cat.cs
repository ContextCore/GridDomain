using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Node.Tests
{
    public class Cat : IAggregate
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
                    return new Feeded(Id){Version = Version}.AsCommandResult();
                case GetNewCatCommand c:
                    return new Born(c.Recipient.Id){Version = Version}.AsCommandResult();
                case FeelHungryCommand c:
                    return new GotHungry(Id){Version = Version}.AsCommandResult();
                case PetCommand c:
                    switch (Mood)
                    {
                        case Mood.Good: return new Tired(Name){Version = Version}.AsCommandResult();
                        case Mood.Neutral: return new GotHungry(Name){Version = Version}.AsCommandResult();
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
        
        public class PetCommand : Command<Cat>
        {
            public PetCommand(string catName):base(catName)
            {
            }
        }


        
        public class Born: DomainEvent<Cat>
        {
            public string Name => Source.Id;

            public Born(string name) : base(name)
            {
            }
        }
        
        public class GotHungry:DomainEvent<Cat>
        {
            public GotHungry(string name) : base(name)
            {
            }
        }
        
        public class Tired:DomainEvent<Cat>
        {
            public Tired(string name) : base(name)
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
}
