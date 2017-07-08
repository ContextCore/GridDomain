using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    public static class TestFixtureExtensions
    {
       

        public static NodeTestFixture IgnoreCommands(this NodeTestFixture fixture)
        {
            fixture.OnNodeStartedEvent += (sender, e) =>
                                          {
                                              //supress errors raised by commands not reaching aggregates
                                              var nullActor = fixture.Node.System.ActorOf(BlackHoleActor.Props);
                                              fixture.Node.Pipe.SagaProcessor.Ask<Initialized>(new Initialize(nullActor))
                                                     .Wait();
                                          };

            return fixture;
        }
    }
}