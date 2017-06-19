using System;
using Akka.Actor;
using Akka.TestKit.TestActors;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public static class TestFixtureExtensions
    {
        public static void InitFastRecycle(this NodeTestFixture fixture,
                                           TimeSpan? clearPeriod = null,
                                           TimeSpan? maxInactiveTime = null)
        {
            fixture.Add(
                        new CustomContainerConfiguration(
                                                         c =>
                                                             c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                                                                                                                       new PersistentChildsRecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(200),
                                                                                                                                                                maxInactiveTime ?? TimeSpan.FromMilliseconds(50)))));
        }

        public static NodeTestFixture InitSampleAggregateSnapshots(this NodeTestFixture fixture,
                                                                   int keep = 1,
                                                                   TimeSpan? maxSaveFrequency = null)
        {
            fixture.Add(new AggregateConfiguration<Balloon, BalloonCommandHandler>(
                                                                                   () => new SnapshotsPersistencePolicy(1, keep, maxSaveFrequency)
                                                                                         {
                                                                                             Log = fixture.Logger.ForContext<SnapshotsPersistencePolicy>()
                                                                                         },
                                                                                   Balloon.FromSnapshot));

            return fixture;
        }

        public static NodeTestFixture InitSoftwareProgrammingSagaSnapshots(this NodeTestFixture fixture,
                                                                           int keep = 1,
                                                                           TimeSpan? maxSaveFrequency = null,
                                                                           int saveOnEach = 1)
        {
            var containerConfiguration = new SagaConfiguration<SoftwareProgrammingProcess,
                                                               SoftwareProgrammingState,
                                                               SoftwareProgrammingSagaFactory>
                (SoftwareProgrammingProcess.Descriptor,
                 () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency), null);

            fixture.Add(new CustomContainerConfiguration(c =>{c.Register(containerConfiguration);}));

            return fixture;
        }

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