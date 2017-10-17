using Akka.Actor;

namespace GridDomain.Tests.Unit {
    public static class TestActorSystem
    {
        public static ActorSystem Create(string name = null)
        {
            return ActorSystem.Create(name ?? "test", @"akka.suppress-json-serializer-warning = on
                                                        akka.suppress-json-serializer-warning");
        }
    }
}