using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.EventsUpgrade;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Quartz;
using Wire;

namespace GridDomain.Tests
{
    [TestFixture]
    public class QuartzJobSeserializationTests : InMemorySampleDomainTests
    {
        [Test]
        public void QuartzJob_should_de_deserialized_from_old_wire_format()
        {
            var cmd = new ChangeSampleAggregateCommand(1,Guid.NewGuid());
            var evt = new SampleAggregateCreatedEvent("1", cmd.AggregateId);

            var scheduleKey = ScheduleKey.For(cmd);

            var oldSerializer = new LegacyWireSerializer();

            var serializedEvent = oldSerializer.Serialize(evt);
            var serializedKey = oldSerializer.Serialize(scheduleKey);

            var jobDataMap = new JobDataMap
            {
                { "EventKey", serializedEvent },
                { "ScheduleKey", serializedKey }
            };

            var job = QuartzJob.CreateJob(scheduleKey, jobDataMap);

            var trigger =  TriggerBuilder.Create()
                                         .WithIdentity(job.Key.Name, job.Key.Group)
                                         .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow()
                                                                   .WithRepeatCount(0))
                                         .StartAt(BusinessDateTime.Now.AddSeconds(1))
                                         .Build();

            var scheduler = GridNode.Container.Resolve<IScheduler>();

            scheduler.ScheduleJob(job, trigger);


            var waiter = GridNode.NewWaiter(Timeout)
                                 .Expect<SampleAggregateCreatedEvent>(e => e.SourceId == evt.SourceId)
                                 .Create();
            waiter.Wait();
        }

        [Test]
        public void QuartzJob_should_be_deserialized_from_new_wire_format()
        {
            var cmd = new ChangeSampleAggregateCommand(1, Guid.NewGuid());
            var evt = new SampleAggregateCreatedEvent("1", cmd.AggregateId);

            var scheduleKey = ScheduleKey.For(cmd);

            var oldSerializer = new Serializer(new SerializerOptions(true,true,null,null));

            var streamEvent = new MemoryStream();
             oldSerializer.Serialize(evt,streamEvent);
            var serializedEvent = streamEvent.ToArray();

            var streamKey = new MemoryStream();
            oldSerializer.Serialize(scheduleKey, streamKey);
            var serializedKey = streamKey.ToArray();

            var jobDataMap = new JobDataMap
            {
                { "EventKey", serializedEvent },
                { "ScheduleKey", serializedKey }
            };

            var job = QuartzJob.CreateJob(scheduleKey, jobDataMap);

            var trigger = TriggerBuilder.Create()
                                         .WithIdentity(job.Key.Name, job.Key.Group)
                                         .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow()
                                                                   .WithRepeatCount(0))
                                         .StartAt(BusinessDateTime.Now.AddSeconds(1))
                                         .Build();

            var scheduler = GridNode.Container.Resolve<IScheduler>();

            scheduler.ScheduleJob(job, trigger);


            var waiter = GridNode.NewWaiter(Timeout)
                                 .Expect<SampleAggregateCreatedEvent>(e => e.SourceId == evt.SourceId)
                                 .Create();
            waiter.Wait();
        }

        [Test]
        public void QuartzJob_should_be_deserialized_from_already_persisted_job()
        {
            var bytesString =
                "0x0001000000FFFFFFFF01000000000000000C020000004951756172747A2C2056657273696F6E3D322E332E332E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6636623863393861343032636338613405010000001151756172747A2E4A6F62446174614D6170030000000776657273696F6E056469727479036D61700000030801E20153797374656D2E436F6C6C656374696F6E732E47656E657269632E44696374696F6E61727960325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D02000000010000000109030000000403000000E20153797374656D2E436F6C6C656374696F6E732E47656E657269632E44696374696F6E61727960325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D040000000756657273696F6E08436F6D7061726572084861736853697A650D4B657956616C756550616972730003000308920153797374656D2E436F6C6C656374696F6E732E47656E657269632E47656E65726963457175616C697479436F6D706172657260315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D08E60153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D5B5D0300000009040000001100000009050000000404000000920153797374656D2E436F6C6C656374696F6E732E47656E657269632E47656E65726963457175616C697479436F6D706172657260315B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D00000000070500000000010000000300000003E40153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D04FAFFFFFFE40153797374656D2E436F6C6C656374696F6E732E47656E657269632E4B657956616C75655061697260325B5B53797374656D2E537472696E672C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D2C5B53797374656D2E4F626A6563742C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038395D5D02000000036B65790576616C7565010206070000000A436F6D6D616E644B6579090800000001F7FFFFFFFAFFFFFF060A0000000B5363686564756C654B6579090B00000001F4FFFFFFFAFFFFFF060D00000013457865637574696F6E4F7074696F6E734B6579090E0000000F08000000F200000002FFA500000047726964446F6D61696E2E4576656E74536F757263696E672E53616761732E4675747572654576656E74732E52616973655363686564756C6564446F6D61696E4576656E74436F6D6D616E642C2047726964446F6D61696E2E4576656E74536F757263696E672E53616761732C2056657273696F6E3D312E362E3231362E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C7303000091C08140AC491D5280C144667AF36B3EB86C0A4FBCC5BFC112C3DD9606EF1FE8B8F1644D906A0C7AC047DC6700000000000000000000000000000000C6CA4CD708E3D3080F0B0000003E01000002FF7F00000047726964446F6D61696E2E5363686564756C696E672E416B6B612E4D657373616765732E5363686564756C654B65792C2047726964446F6D61696E2E5363686564756C696E672C2056657273696F6E3D312E362E3231362E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C000000002A000000427573696E657373537562736372697074696F6E4167677265676174655F6675747572654576656E74737AF36B3EB86C0A4FBCC5BFC112C3DD9674000000427573696E657373537562736372697074696F6E4167677265676174655F30303030303337332D633039312D343038312D616334392D3164353238306331343436365F6675747572655F6576656E745F33653662663337612D366362382D346630612D626363352D6266633131326333646439360F0E000000A001000002FF8400000047726964446F6D61696E2E5363686564756C696E672E416B6B612E4D657373616765732E457865637574696F6E4F7074696F6E732C2047726964446F6D61696E2E5363686564756C696E672C2056657273696F6E3D312E362E3231362E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6C3FA8E1EA8A2AD40810A1000000536F6C6F6D6F746F2E4D656D626572736869702E446F6D61696E2E4576656E74732E427573696E657373537562736372697074696F6E2E496E616374697665506572696F6441646465644576656E742C20536F6C6F6D6F746F2E42616C616E63652E446F6D61696E2C2056657273696F6E3D312E302E302E3434392C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D6E756C6CFF5C00000053797374656D2E54696D655370616E2C206D73636F726C69622C2056657273696F6E3D342E302E302E302C2043756C747572653D6E65757472616C2C205075626C69634B6579546F6B656E3D623737613563353631393334653038390046C323000000000B";

            var bytes = Encoding.Unicode.GetBytes(bytesString);


        }
    }

    public static class EventsExtensions
    {
        private static readonly ComparisonConfig StrictConfig = new ComparisonConfig {DoublePrecision = 0.0001};

        private static readonly ComparisonConfig DateCreatedIgnoreConfig = new ComparisonConfig
        {
            MembersToIgnore = new[] {nameof(DomainEvent.CreatedTime)}.ToList(),
            DoublePrecision = 0.0001
        };

        /// <summary>
        ///     Compare events ignoring creation date
        /// </summary>
        /// <param name="expected1"></param>
        /// <param name="published2"></param>
        public static void CompareEvents(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2)
        {
            CompareEventsByLogic(expected1, published2, new CompareLogic {Config = DateCreatedIgnoreConfig});
        }

        private static void CompareEventsByLogic(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2,
            CompareLogic compareLogic)
        {
            var expected = expected1.ToArray();
            var published = published2.ToArray();

            if (expected.Length != published.Length)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Разное количество событий");
                sb.AppendLine($"Ожидается: {expected.Length}, получено: {published.Length}");
                sb.AppendLine("Ожидаемые события:");
                sb.AppendLine(string.Join(";", expected.Select(e => e.ToString())));
                sb.AppendLine("Полученные события:");
                sb.AppendLine(string.Join(";", published.Select(e => e.ToString())));

                Assert.Fail(sb.ToString());
            }

            var eventPairs = expected.Zip(published, (e, p) => new {Expected = e, Produced = p});

            foreach (var events in eventPairs)
            {
                compareLogic.Config.ActualName = events.Produced.GetType().Name;
                compareLogic.Config.ExpectedName = events.Expected.GetType().Name;
                var comparisonResult = compareLogic.Compare(events.Expected, events.Produced);

                if (!comparisonResult.AreEqual)
                {
                    Assert.Fail(comparisonResult.DifferencesString);
                }
            }
        }

        public static void CompareEventsStrict(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2)
        {
            CompareEventsByLogic(expected1, published2, new CompareLogic {Config = StrictConfig});
        }
    }
}