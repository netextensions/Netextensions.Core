using System.Threading;
using System.Threading.Tasks;

namespace Netextensions.Core.Processor
{
    public class ChainedProcessor<T> : IProcessor<T>  
    {
        private IProcessor<T> Inner { get; }
        private IProcessor<T> Next { get; }

        public ChainedProcessor(IProcessor<T> inner, IProcessor<T> next)
        {
            Inner = inner;
            Next = next;
        }

        public async Task<T> ExecuteAsync(T payload, CancellationToken token) =>
            await Next.ExecuteAsync(await Inner.ExecuteAsync(payload, token).ConfigureAwait(false), token)
                .ConfigureAwait(false);
    }
}