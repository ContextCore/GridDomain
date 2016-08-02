namespace GridDomain.Logging
{
    public interface ILoggerFactory
    {
         ISoloLogger GetLogger(string className = null);
    }
}