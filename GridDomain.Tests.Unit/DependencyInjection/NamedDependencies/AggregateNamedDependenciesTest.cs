using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.Xunit2;
using Microsoft.Practices.Unity;
using Xunit;

namespace GridDomain.Tests.Unit.DependencyInjection.NamedDependencies
{
    public class AggregateNamedDependenciesTest : TestKit
    {
        [Fact]
        public void Actors_should_resolve_named_dependencies()
        {
            var container = new UnityContainer();
            container.RegisterInstance("A", new SomeService("A"));
            container.RegisterInstance("B", new SomeService("B"));
            container.RegisterType<NamedActorA>(new InjectionFactory(c => new NamedActorA(c.Resolve<SomeService>("A"))));
            container.RegisterType<NamedActorB>(new InjectionFactory(c => new NamedActorB(c.Resolve<SomeService>("B"))));
            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));

            var actorA = ActorOfAsTestActorRef<NamedActorA>(Sys.DI().Props<NamedActorA>());
            var actorB = ActorOfAsTestActorRef<NamedActorB>(Sys.DI().Props<NamedActorB>());

            Assert.Equal("A", actorA.UnderlyingActor.Dep.Name);
            Assert.Equal("B", actorB.UnderlyingActor.Dep.Name);
        }
    }
}