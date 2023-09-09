namespace RegressionBuilder
{
    public class RInterval
    {
        public RPoint LowerBound;
        public RPoint UpperBound;

        public decimal YDelta;

        public RInterval()
        :this(new RPoint(),new RPoint())
        {
            
        }
        public RInterval(RPoint lowerBound, RPoint upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;

            YDelta = upperBound.Y - lowerBound.Y;
        }
    }
}
