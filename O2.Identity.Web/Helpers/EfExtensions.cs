using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace O2.Identity.Web.Helpers
{
    // public class AsyncEnumerableQuery<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T> {
    //     public AsyncEnumerableQuery(IEnumerable<T> enumerable) : base(enumerable) {
    //     }
    //
    //     public AsyncEnumerableQuery(Expression expression) : base(expression) {
    //     }
    //
    //     public IDbAsyncEnumerator<T> GetAsyncEnumerator() {
    //         return new InMemoryDbAsyncEnumerator<T>(((IEnumerable<T>) this).GetEnumerator());
    //     }
    //
    //     IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() {
    //         return GetAsyncEnumerator();
    //     }
    //
    //     private class InMemoryDbAsyncEnumerator<T> : IDbAsyncEnumerator<T> {
    //         private readonly IEnumerator<T> _enumerator;
    //
    //         public InMemoryDbAsyncEnumerator(IEnumerator<T> enumerator) {
    //             _enumerator = enumerator;
    //         }
    //
    //         public void Dispose() {
    //         }
    //
    //         public Task<bool> MoveNextAsync(CancellationToken cancellationToken) {
    //             return Task.FromResult(_enumerator.MoveNext());
    //         }
    //
    //         public T Current => _enumerator.Current;
    //
    //         object IDbAsyncEnumerator.Current => Current;
    //     }
    // }
    //
    // public static class EfExtensions
    // {
    //     public static Task<List<TSource>> ToListAsyncSafe<TSource>(
    //         this IQueryable<TSource> source)
    //     {
    //         if (source == null)
    //             throw new ArgumentNullException(nameof(source));
    //         if (!(source is IAsyncEnumerable<TSource>))
    //             return Task.FromResult(source.ToList());
    //         return source.ToListAsync();
    //     }
    //     
    // }
}