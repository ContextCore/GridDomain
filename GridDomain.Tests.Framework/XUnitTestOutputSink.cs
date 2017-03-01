using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Xunit.Abstractions;

namespace GridDomain.Tests.Framework
{
    public class XUnitTestOutputSink : ILogEventSink
    {
        private readonly ITestOutputHelper _output;
        private readonly ITextFormatter _textFormatter;

        public XUnitTestOutputSink(ITestOutputHelper testOutputHelper, ITextFormatter textFormatter)
        {
            if (testOutputHelper == null)
                throw new ArgumentNullException(nameof(testOutputHelper));
            if (textFormatter == null)
                throw new ArgumentNullException(nameof(textFormatter));

            _output = testOutputHelper;
            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            _output.WriteLine(renderSpace.ToString());
        }
    }
}