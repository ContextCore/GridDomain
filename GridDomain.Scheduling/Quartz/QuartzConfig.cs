namespace GridDomain.Scheduling.Quartz
{
    public class QuartzConfig : IQuartzConfig
    {
        public string ConnectionString => "Integrated Security=true;Database=Quartz;MultipleActiveResultSets=True;Application Name=Quartz;";
    }
}