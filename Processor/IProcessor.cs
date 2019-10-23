using System.Threading;
using System.Threading.Tasks;

namespace Netextensions.Core.Processor
{
    public interface IProcessor<T>
    {
        Task<T> ExecuteAsync(T load, CancellationToken token);
    }
}
