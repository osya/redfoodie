using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace redfoodie.Tests
{
    internal class AsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(Expression expression) : base(expression) { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() => new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() => GetAsyncEnumerator();

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
    }
}