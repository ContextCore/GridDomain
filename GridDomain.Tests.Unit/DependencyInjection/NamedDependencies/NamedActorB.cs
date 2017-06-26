using Akka.Actor;

namespace GridDomain.Tests.Unit.DependencyInjection.NamedDependencies
{
    public class NamedActorB : ReceiveActor
    {
        public readonly SomeService Dep;

        public NamedActorB(SomeService dep)
        {
            Dep = dep;
        }
    }
}