using System.Threading.Tasks;

namespace DocumentApi.Infrastructure
{
    interface IStartupInitializer
    {
        Task InitAsync();
    }
}
