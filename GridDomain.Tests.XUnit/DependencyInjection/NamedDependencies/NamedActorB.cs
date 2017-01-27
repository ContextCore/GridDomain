using Akka.Actor;

namespace GridDomain.Tests.XUnit.DependencyInjection.NamedDependencies
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