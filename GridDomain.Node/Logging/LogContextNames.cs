using Serilog.Core;

namespace GridDomain.Node.Logging {
    public static class LogContextNames
    {
        public const string Timestamp = "Timestamp";
        public const string ClassShortName = "ClassName";
        public const string Class = "Class";
        public const string SourceContext = Constants.SourceContextPropertyName;
        public const string Path = "Path";
        public const string Thread = "Thread";
    }
}