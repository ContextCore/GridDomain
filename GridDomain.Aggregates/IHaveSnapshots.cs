namespace GridDomain.Aggregates
{
    public interface IHaveSnapshots
    {
        ISnapshot Take();
        void Restore(ISnapshot snapshot);
    }
}