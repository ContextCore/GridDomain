namespace Shop.Infrastructure
{
    /// <summary>
    ///provides unique, increasing, application-wide, persisted between app restarts numbers sequence
    ///used for id generation and human-readable numbers generations  
    /// </summary>
    public interface ISequenceProvider
    {
        long GetNext(string sequenceName = null);
    }
}