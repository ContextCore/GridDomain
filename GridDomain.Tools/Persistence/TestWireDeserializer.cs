//using System;
//using System.IO;
//using Akka.Actor;
//using Akka.Serialization;
//using Akka.TestKit.NUnit;
//using GridDomain.Tests.Framework.Configuration;
//using NUnit.Framework;

//namespace Solomoto.Membership.Tools
//{
//    [TestFixture]
//    public class TestWireDeserializer:TestKit
//    {

//        public TestWireDeserializer():base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig())
//        {
            
//        }

//        [Test]
//        public void Wire_serializer_as_in_akka()
//        {
//            var purchasedSubscriptionActivated = new PurchasedSubscriptionActivated(Guid.NewGuid(), Guid.NewGuid(), new AccountTransfer(Guid.NewGuid(), Guid.NewGuid(), new Money(100)));
//            var _serializer = new Wire.Serializer(new SerializerOptions(
//                preserveObjectReferences: true,
//                versionTolerance: true));

//            byte[] bytes = null;
//            using (var ms = new MemoryStream())
//            {
//                _serializer.Serialize(purchasedSubscriptionActivated, ms);
//                bytes = ms.ToArray();
//            }

//            Console.WriteLine(ByteArrayToString(bytes));
//        }

//        [Test]
//        public void Test_Akka_serialization()
//        {
//            var purchasedSubscriptionActivated = new PurchasedSubscriptionActivated(Guid.NewGuid(), Guid.NewGuid(), new AccountTransfer(Guid.NewGuid(), Guid.NewGuid(), new Money(100)));

//            var serialization = new Serialization((ExtendedActorSystem)Sys);
//            var serializer = serialization.FindSerializerFor(purchasedSubscriptionActivated);

//            var bytes = serializer.ToBinary(purchasedSubscriptionActivated);

//            Console.WriteLine(ByteArrayToString(bytes));
//        }


     
//        public void Test_WireDeserializer_type(string content)
//        {
//            //  var serializer = new Wire.Serializer(new SerializerOptions());
//            //  var bytes = StringToByteArray(content);
//            //  (serializer.Deserialize<PurchasedSubscriptionActivated>(new MemoryStream(bytes)));

//            var deserializer = new SqlJournalWireDeserializer();
//            object obj = deserializer.Deserialize<PurchasedSubscriptionActivated>(content);//
//            Assert.NotNull(obj);
//            Console.WriteLine(obj.ToPropsString());
//        }

//        [TestCase(JournalEntry_SubscriptionCreated.Payload, Description = "Subscription created")]
//        [TestCase(JournalEntry_PeriodActivated.Payload, Description = "Period activated")]
//        [TestCase(JournalEntry_PurchasedSubscriptioActivated.Payload, Description = "SubscriptionActivated")]
//        [TestCase(JournalEntry_FutureEvent.Payload, Description = "Future event")]
//        public void Test_existing_payload_deserialization_by_custom_wire(string content)
//        {
//            Func<byte[], object> deserializer = b =>
//            {
//                var serializer = new Wire.Serializer(new SerializerOptions());
//                return serializer.Deserialize<PurchasedSubscriptionActivated>(new MemoryStream(b));
//            };

//            Test_WireDeserializer_type(content);
//        }
//        [TestCase(JournalEntry_SubscriptionCreated.Payload, Description = "Subscription created")]
//        [TestCase(JournalEntry_PeriodActivated.Payload, Description = "Period activated")]
//        [TestCase(JournalEntry_PurchasedSubscriptioActivated.Payload, Description = "SubscriptionActivated")]
//        [TestCase(JournalEntry_FutureEvent.Payload, Description = "Future event")]
//        public void Test_existing_payload_deserialization_like_akka_wire(string content)
//        {
//            Func<byte[], object> deserializer = b =>
//            {
//                var _serializer = new Wire.Serializer(new SerializerOptions(
//                    preserveObjectReferences: true,
//                    versionTolerance: true));

//                return _serializer.Deserialize<PurchasedSubscriptionActivated>(new MemoryStream(b));
//            };

//            Test_WireDeserializer_type(content);
//        }





//    }
//}