using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static T FirstOrNull<T>(this IEnumerable<T> values, Func<T, bool> predicate) where T: class
        {
            return values.DefaultIfEmpty(null).FirstOrDefault(predicate);
        }
    }
}
