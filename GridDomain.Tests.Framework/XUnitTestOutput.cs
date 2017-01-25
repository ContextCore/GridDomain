namespace Serilog.Sinks.XunitTestOutput
{
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Formatting;
    using System;
    using System.IO;
    using Xunit.Abstractions;

    public class XUnitTestOutputSink : ILogEventSink
    {
        readonly ITestOutputHelper _output;
        readonly ITextFormatter _textFormatter;

        public XUnitTestOutputSink(ITestOutputHelper testOutputHelper, ITextFormatter textFormatter)
        {
            if (testOutputHelper == null) throw new ArgumentNullException(nameof(testOutputHelper));
            if (textFormatter == null) throw new ArgumentNullException(nameof(textFormatter));

            _output = testOutputHelper;
            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            _output.WriteLine(renderSpace.ToString());
        }
    }
}