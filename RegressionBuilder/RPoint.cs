namespace RegressionBuilder
{
    public class RPoint
    {
        public decimal X { get; private set; }
        public decimal Y { get; private set;}

        public RPoint(decimal x, decimal y)
        {
            Set(x, y);
        }

        public RPoint()
        {
            X = default;
            Y = default;
        }
        public void Set(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public void Copy(RPoint point)
        {
            X = point.X;
            Y = point.Y;
        }
    }
}
