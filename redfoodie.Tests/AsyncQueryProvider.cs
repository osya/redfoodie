using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace redfoodie.Tests
{
    internal class AsyncQueryProvider<T> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal AsyncQueryProvider(IQueryProvider provider)
        {
            _inner = provider;
        }

        public IQueryable CreateQuery(Expression expression) => new AsyncEnumerable<T>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute(expression));

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));
    }
}