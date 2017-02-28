using System;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using Debug = System.Diagnostics.Debug;

namespace GridGomain.Tests.Stress
{
    internal class TestLogActor : ReceiveActor

    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public TestLogActor()
        {
            _log.Debug("actor created debug");
            _log.Info("actor info");
            _log.Error("actor error");
            _log.Warning("actor warn");

            Console.WriteLine("Hi from console");
            Debug.Print("Try debug write");
            StandardOutWriter.WriteLine("Hi from standart out writer");

            ReceiveAny(o =>
                       {
                           Console.WriteLine("Hi from console on receive");

                           _log.Debug("Debug: received " + o);
                           _log.Info("Info: received " + o);
                           _log.Error("Error: received " + o);
                           _log.Warning("Warning: received " + o);
                           Sender.Tell(o);
                       });
        }
    }
}