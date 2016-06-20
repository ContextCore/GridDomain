namespace GridDomain.Scheduling.Integration
{
    public class DefaultLoggerFactory : LoggerFactory
    {
        public override ISoloLogger GetLogger()
        {
            return new DefaultSoloLogger();
        }
    }
}