using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.TestKit.Xunit2;
using Autofac;
using Xunit;

namespace GridDomain.Tests.Unit.DependencyInjection.NamedDependencies
{
    public class AggregateNamedDependenciesTest : TestKit
    {
        [Fact]
        public void Actors_should_resolve_named_dependencies()
        {
            var container = new ContainerBuilder();
            container.RegisterInstance(new SomeService("A")).Named<SomeService>("A");
            container.RegisterInstance(new SomeService("B")).Named<SomeService>("B");
            container.Register<NamedActorA>(c => new NamedActorA(c.ResolveNamed<SomeService>("A")));
            container.Register<NamedActorB>(c => new NamedActorB(c.ResolveNamed<SomeService>("B")));
            Sys.AddDependencyResolver(new AutoFacDependencyResolver(container.Build(), Sys));

            var actorA = ActorOfAsTestActorRef<NamedActorA>(Sys.DI().Props<NamedActorA>());
            var actorB = ActorOfAsTestActorRef<NamedActorB>(Sys.DI().Props<NamedActorB>());

            Assert.Equal("A", actorA.UnderlyingActor.Dep.Name);
            Assert.Equal("B", actorB.UnderlyingActor.Dep.Name);
        }
    }
}