namespace System
{
    public static class MathExtensions
    {
        public static T Clamp<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            if (value.CompareTo(minimum) < 0) return minimum;
            return value.CompareTo(maximum) > 0 ? maximum : value;
        }
    }
}
