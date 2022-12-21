namespace Billing.LinqExtensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> TakeLong<T>(this IEnumerable<T> longs, long count)
        {
            foreach (var item in longs)
            {
                if (count <= 0)
                    yield break;
                count--;
                yield return item;
            }
        }

        public static long CountLong<T>(this IEnumerable<T> longs)
        {
            long count = 0;
            foreach (var item in longs)
                count++;
            return count;
        }
    }
}
