using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static T FirstOrNull<T>(this IEnumerable<T> Values, Func<T, bool> Predicate) where T: class
        {
            return Values.DefaultIfEmpty(null).FirstOrDefault(Predicate);
        }
    }
}
