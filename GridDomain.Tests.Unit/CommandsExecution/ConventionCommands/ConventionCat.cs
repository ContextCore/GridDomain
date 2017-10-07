using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.CommandsExecution.ConventionCommands
{
    public enum FoodKind
    {
        Fish,
        Chicken,
        Mouse
    }

    class GiveFoodCommand:Command
    {
        public GiveFoodCommand(Guid catId,FoodKind kind, int grammsWeight):base(catId)
        {
            Kind = kind;
            GrammsWeight = grammsWeight;
        }
        public int GrammsWeight { get; }
        public FoodKind Kind { get; }
    }

    class ConventionCat : CommandAggregate
    {
        public int hungryLevel;
        private Dictionary<FoodKind,bool> FoodLikeMap = new Dictionary<FoodKind, bool>();
        public void Consume(FoodKind food, int gramms)
        {
        }

      // protected ConventionCat(Guid id) : base(id)
      // {
      //     Apply<>();
      //     Execute<GiveFoodCommand>(c => );
      // }
      //
      // public ConventionCat Create(NewCatCommand)
      // public Task Execute(GiveFoodCommand c)
      // {
      //     
      // }
        protected ConventionCat(Guid id) : base(id) { }
    }
}
