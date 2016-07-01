namespace GridDomain.Logging
{
    public class DefaultLoggerFactory : LoggerFactory
    {
        public override ISoloLogger GetLogger()
        {
            return new SerilogLogger();
        }
    }
}