using Akka.Actor;

namespace GridDomain.Tests.XUnit.DependencyInjection.NamedDependencies
{
    public class NamedActorA : ReceiveActor
    {
        public readonly SomeService Dep;

        public NamedActorA(SomeService dep)
        {
            Dep = dep;
        }
    }
}