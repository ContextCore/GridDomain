
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Serialization {
    public class XUnitTraceWriter : ITraceWriter
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XUnitTraceWriter(ITestOutputHelper helper)
        {
            _testOutputHelper = helper;
        }
        public TraceLevel LevelFilter => TraceLevel.Verbose;

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            _testOutputHelper.WriteLine(message);
        }
    }
}