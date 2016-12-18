using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace redfoodie.Tests
{
    internal class AsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> enumerator)
        {
            _inner = enumerator;
        }

        public void Dispose() => _inner.Dispose();

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) => Task.FromResult(_inner.MoveNext());

        public T Current => _inner.Current;

        object IDbAsyncEnumerator.Current => Current;
    }
}