using System.Threading.Tasks;

namespace GridDomain.CQRS {
    public static class WaitResultExtensions
    {
        public static async Task<T> Received<T>(this Task<IWaitResult<T>> res) where T : class
        {
            return (await res).Received;
        }
    }
}