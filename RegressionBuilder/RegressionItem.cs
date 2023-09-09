using System;
using System.Collections.Generic;

namespace RegressionBuilder
{
    public class RegressionItem:ICloneable
    {
        public RPoint RegressionPoint { get; }
        /// <summary>
        /// Prediction Interval
        /// </summary>
        public RInterval PI { get; }
        /// <summary>
        /// Confidence Interval
        /// </summary>
        public RInterval CI { get; }

        public decimal CIValue => CI.YDelta;
        public decimal PIValue => PI.YDelta;

        public bool Arbitrary { get; }


        // ReSharper disable once UnusedMember.Global
        public RegressionItem()
        :this(new RPoint(),new RInterval(),new RInterval(),false)
        {
            
        }

        public RegressionItem(decimal x, decimal y, decimal confidenceInterval, decimal predictionInterval,bool arbitrary)
        {
            RegressionPoint = new RPoint(x, y);
            CI= new RInterval(new RPoint(x, y - confidenceInterval), new RPoint(x, y + confidenceInterval));
            PI= new RInterval(new RPoint(x, y - predictionInterval), new RPoint(x, y + predictionInterval));
            
            Arbitrary = arbitrary;
        }

       

        public RegressionItem(RPoint regressionPoint, RInterval confidenceInterval, RInterval predictionInterval,bool arbitrary)
        {
            RegressionPoint = regressionPoint;
            CI = confidenceInterval;
            PI = predictionInterval;
            Arbitrary = arbitrary;
        }

        public object Clone()
        {
            return new RegressionItem(RegressionPoint.X, RegressionPoint.Y, CIValue, PIValue, Arbitrary);
        }

        public List<string> GetNames()
        {
            var result = new List<string>
            {
                "X",
                "Y",
                "Prediction interval, upper bound Y",
                "Prediction interval, lower bound Y",
                "Confidence interval, upper bound Y",
                "Confidence interval, lower bound Y", 
                "Prediction value", 
                "Confidence value"
            };

            return result;

        }

        public List<string> ToString(int precision)
        {
            var result = new List<string>
            {
                $@"{Math.Round(RegressionPoint.X, precision):G29}",
                $@"{Math.Round(RegressionPoint.Y, precision):G29}",
                $@"{Math.Round(PI.UpperBound.Y, precision):G29}",
                $@"{Math.Round(PI.LowerBound.Y, precision):G29}",
                $@"{Math.Round(CI.UpperBound.Y, precision):G29}",
                $@"{Math.Round(CI.LowerBound.Y, precision):G29}",
                $@"{Math.Round(PIValue, precision):G29}",
                $@"{Math.Round(CIValue, precision):G29}"
            };

            return result;
        }
    }
}
