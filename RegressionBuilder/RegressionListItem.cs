namespace RegressionBuilder
{
    public class RegressionListItem
    {
        public decimal X;
        public decimal Y;
        public decimal CILowerBoundX;
        public decimal CILowerBoundY;
        public decimal CIUpperBoundX;
        public decimal CIUpperBoundY;
        public decimal CIDelta;
        public decimal PILowerBoundX;
        public decimal PILowerBoundY;
        public decimal PIUpperBoundX;
        public decimal PIUpperBoundY;
        public decimal PIDelta;

        public RegressionListItem()
        {
           
        }

        public RegressionListItem(RegressionItem item)
        {
            var rPoint = item.RegressionPoint;
            var ciInterval = item.CI;
            var piInterval = item.PI;

            X = rPoint.X;
            Y = rPoint.Y;
            CILowerBoundX = ciInterval.LowerBound.X;
            CILowerBoundY = ciInterval.LowerBound.Y;

            CIUpperBoundX = ciInterval.UpperBound.X;
            CIUpperBoundY = ciInterval.UpperBound.Y;

            CIDelta = ciInterval.YDelta;

            PILowerBoundX = piInterval.LowerBound.X;
            PILowerBoundY = piInterval.LowerBound.Y;

            PIUpperBoundX = piInterval.UpperBound.X;
            PIUpperBoundY = piInterval.UpperBound.Y;

            PIDelta = piInterval.YDelta;
        }
    }
}
