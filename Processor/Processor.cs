using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Netextensions.Core.Processor
{
    public abstract class Processor<T> : IProcessor<T>
    {
        private static IProcessor<T> LoadProcessors(IEnumerable<IProcessor<T>> processors) => processors.Aggregate((current, next) => current.Then(next));
        protected abstract IEnumerable<IProcessor<T>> GetProcessor(T payload);
        public async Task<T> ExecuteAsync(T load, CancellationToken token) => await LoadProcessors(GetProcessor(load)).ExecuteAsync(load, token).ConfigureAwait(false);
     }
}