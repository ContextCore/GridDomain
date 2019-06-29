namespace GridDomain.Abstractions
{
    public interface IDomain
    {
        T GetPart<T>() where T : IDomainPart;
    }
}