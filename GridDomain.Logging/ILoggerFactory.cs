namespace GridDomain.Logging
{
    public interface ILoggerFactory
    {
         ISoloLogger GetLogger();
    }
}