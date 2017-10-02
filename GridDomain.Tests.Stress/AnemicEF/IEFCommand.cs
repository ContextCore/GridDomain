using System.Threading.Tasks;

namespace GridDomain.Tests.Stress.AnemicEF {
    interface IEFCommand
    {
        Task Execute();
    }
}