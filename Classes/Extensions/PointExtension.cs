namespace System.Drawing
{
    public static class PointExtensions
    {
        public static Point Negate(this Point p)
        {
            return new Point(-p.X, -p.Y);
        }
    }
}
