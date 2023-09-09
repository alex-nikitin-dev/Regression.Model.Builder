using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace RegressionBuilder
{
    class Regression
        :ICloneable
    {
        #region .ctor

        private Regression(RelatedSample initialSample, 
            RelatedSample regressionSample, 
            decimal b0, decimal b1, 
            RegressionValuesCollection regressionValues,
            Regression nonLinearRegression, 
            RegressionType regressionType,
            SampleNormalizationType normalizationType)
        {
            B0 = b0;
            B1 = b1;
            InitialSample = initialSample;
            RegressionSample = regressionSample;
            RegressionValues = regressionValues;
            NonLinearRegression = nonLinearRegression;
            RegressionType = regressionType;
            NormalizationType = normalizationType;
        }

        public Regression(RelatedSample sample)
        {
            RegressionValues = new RegressionValuesCollection();
            InitialSample = (RelatedSample) sample.Clone();
            RegressionSample = (RelatedSample) sample.Clone();
            InitialSample.SortByX();
            NonLinearRegression = null;
            RegressionType = RegressionType.None;
            NormalizationType = sample.NormalizationType;
        }

        #endregion

        #region .Properties

        public RegressionType RegressionType { get; private set; }
        /// <summary>
        /// With what normalization has been constructed the Regression
        /// </summary>
        public SampleNormalizationType NormalizationType { get; private set; }
        public decimal B0 { get; private set; }
        public decimal B1 { get; private set; }

        public RelatedSample InitialSample;

        public RelatedSample RegressionSample { get; }


        //Replace this property with just a List<decimal> PI and a CI. 
        //Add this[] property to access "Regression values" -> i.e. return 
        //a tuple of the values needed to show the regression.
        public RegressionValuesCollection RegressionValues { get; }

        public Regression NonLinearRegression { get; private set; }

        public string FullName
        {
            get
            {
                var name = $"{RegressionType.GetDisplayName()} ({NormalizationType.GetDisplayName()})";
                return name;
            }
        }

        #endregion

        #region Construction
        public void CalculateAndRemoveOutliers(bool once = true, bool calculateNonLinear = true, bool suppressExceptions = false)
        {
            do
            {
                OneStepOfTheCalculation();
            } while (DeleteOutliers() && !once);

            if (calculateNonLinear)
                NonLinearRegression = GetNonLinear(suppressExceptions);

            RegressionType = RegressionType.Linear;
        }


        private void OneStepOfTheCalculation()
        {
            CalculateAndSetB();
            SetLinearRegression();
            SetIntervals();
        }


        private void SetLinearRegression()
        {
            for (int i = 0; i < RegressionSample.Count; i++)
            {
                RegressionSample.Y[i] = Linear(RegressionSample.X[i]);
            }
        }
        

        public decimal Linear(decimal x)
        {
            return B1 * x + B0;
        }

        #endregion

        #region Non-Linear

        private RInterval ReverseRInterval(RInterval interval, decimal reversedX)
        {
            var lowerBoundY = interval.LowerBound.Y;
            var upperBoundY = interval.UpperBound.Y;
            var reversedLowerBoundY = RegressionSample.Y.ReverseValue(lowerBoundY);
            var reversedUpperBoundY = RegressionSample.Y.ReverseValue(upperBoundY);

            return new RInterval(new RPoint(reversedX, reversedLowerBoundY),
                new RPoint(reversedX, reversedUpperBoundY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suppressExceptions"></param>
        /// <returns></returns>
        public Regression GetNonLinear(bool suppressExceptions = false)
        {
            var result = CloneWithEmptyRegressionValues();

            try
            {
                result.InitialSample.ReverseNormalization();
                result.RegressionSample.ReverseNormalization();
                result.NonLinearRegression = null;
            }
            catch (Exception)
            {
                if (suppressExceptions)
                    return null;
                throw;
            }
           

            for (int i = 0; i < RegressionSample.Count; i++)
            {
                var reversed = result.RegressionSample[i];
                var reversedCI = ReverseRInterval(RegressionValues[i].CI, reversed.X);
                var reversedPI = ReverseRInterval(RegressionValues[i].PI, reversed.X);
                var reversedRPoint = new RPoint(reversed.X, reversed.Y);

                result.RegressionValues.Add(new RegressionItem(reversedRPoint, reversedCI, reversedPI,
                    RegressionValues[i].Arbitrary));
            }

            result.RegressionType = RegressionType.NonLinear;
            return result;
        }

        //public decimal NonLinear(decimal x)
        //{
        //   // var k = B0 + 
        //}

        #endregion

        #region Clone

        public object Clone()
        {
            return new Regression((RelatedSample)InitialSample.Clone(),
                (RelatedSample)RegressionSample.Clone(),
                B0,
                B1,
                (RegressionValuesCollection)RegressionValues.Clone(),
                (Regression)NonLinearRegression.Clone(),
                RegressionType,
                NormalizationType);
        }

        private Regression CloneWithEmptyRegressionValues()
        {
            return new Regression((RelatedSample)InitialSample.Clone(),
                (RelatedSample)RegressionSample.Clone(),
                B0,
                B1,
                new RegressionValuesCollection(),
                null,
                RegressionType,
                NormalizationType);
        }

        #endregion

        #region Outliers

        private bool DeleteOutliers()
        {
            bool isThereAny = false;
            for (int i = 0; i < InitialSample.Count; i++)
            {
                var predictionInterval = RegressionValues[i].PI;
                if (InitialSample[i].Y > predictionInterval.UpperBound.Y ^ InitialSample[i].Y < predictionInterval.LowerBound.Y)
                {
                    isThereAny = true;
                    InitialSample.RemoveAt(i);
                    RegressionValues.RemoveAt(i);
                    RegressionSample.RemoveAt(i);
                    i--;
                }
            }

            return isThereAny;
        }

        //private bool IsThereOutliers()
        //{
        //    for (int i = 0; i < InitialSample.Count; i++)
        //    {
        //        var predictionInterval = RegressionValues[i].PI;
        //        if (InitialSample[i].Y > predictionInterval.UpperBound.Y ^ InitialSample[i].Y < predictionInterval.LowerBound.Y)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        #endregion

        #region Intervals

        
        private decimal GetTDistribution(int degreesOfFreedomNumber, double alpha = 0.05)
        {
          
            return (decimal) new Chart().DataManipulator.Statistics.InverseTDistribution(alpha / 2.0, degreesOfFreedomNumber);
        }
        private decimal GetS()
        {
            var sum = 0M;
            for (int i = 0; i < RegressionSample.Count; i++)
            {
                var diff = InitialSample.Y[i] - RegressionSample.Y[i];
                sum += diff * diff;
            }

            return (decimal)Math.Sqrt((double) (sum / (RegressionSample.Count - 2)));
        }
        
        private decimal GetInterval(decimal x, decimal s, decimal t, int n, decimal sumSquaredDiffXAndMean, decimal xSampleMean,bool isItPrediction)
        {
            var diff = x - xSampleMean;
            var p = isItPrediction ? 1 : 0;
            return t * s * (decimal)Math.Sqrt((double)(p +  1M / n + diff * diff / sumSquaredDiffXAndMean));
        }

        /// <summary>
        /// get prediction interval
        /// </summary>
        /// <param name="x">normalized x</param>
        /// <param name="s"></param>
        /// <param name="tDistribution"></param>
        /// <param name="sumSquaredDiffXAndMean"></param>
        /// <param name="meanNormalizedX"></param>
        private decimal GetPredictionInterval(decimal x,decimal s, decimal tDistribution, decimal sumSquaredDiffXAndMean, decimal meanNormalizedX)
        {
            return GetInterval(x, s, tDistribution, InitialSample.Count, sumSquaredDiffXAndMean, meanNormalizedX, true);
        }
        private decimal GetPredictionInterval(decimal x)
        {
            return GetInterval(x, 
                GetS(),
                GetTDistribution(InitialSample.X.Count - 2),
                InitialSample.X.Count,
                (decimal)InitialSample.X.GetSumSquaredDiffXAndSampleMean(),
                InitialSample.X.GetMean(), 
                true);
        }
        /// <summary>
        /// get confidence interval
        /// </summary>
        /// <param name="x">normalized x</param>
        /// <param name="s"></param>
        /// <param name="tDistribution"></param>
        /// <param name="sumSquaredDiffXAndMean"></param>
        /// <param name="meanNormalizedX"></param>
        private decimal GetConfidenceInterval(decimal x,decimal s, decimal tDistribution, decimal sumSquaredDiffXAndMean, decimal meanNormalizedX)
        {
            return GetInterval(x, s, tDistribution, InitialSample.Count, sumSquaredDiffXAndMean, meanNormalizedX, false);
        }

        private decimal GetConfidenceInterval(decimal x)
        {
            return GetInterval(x, 
                GetS(),
                GetTDistribution(InitialSample.X.Count - 2),
                InitialSample.X.Count,
                (decimal)InitialSample.X.GetSumSquaredDiffXAndSampleMean(),
                InitialSample.X.GetMean(), 
                false);
        }

        

        #endregion

        #region Characteristics
        public decimal GetR2()
        {
            decimal sum = 0;

            for (int i = 0; i < RegressionSample.Count; i++)
            {
                var diff = InitialSample.Y[i] - RegressionSample.Y[i];
                sum += diff * diff;
            }

            return 1 - sum / (decimal)RegressionSample.Y.GetSumSquaredDiffXAndSampleMean();
        }


        public List<decimal> GetMRE()
        {
            var result = new List<decimal>();
            for (int i = 0; i < RegressionSample.Count; i++)
            {
                result.Add(Math.Abs((InitialSample.Y[i] - RegressionSample.Y[i]) / InitialSample.Y[i]));
            }

            return result;
        }

        public decimal GetMMRE()
        {
            var mreList = GetMRE();
            return mreList.Sum() / mreList.Count;
        }

        public decimal GetPRED(decimal predicationOfLevel = 0.25M)
        {
            var mreList = GetMRE();
            var mreCountByCriterion = mreList.Count(t => t <= predicationOfLevel);
            return mreCountByCriterion / (decimal) mreList.Count;
        }

        public RelatedSample GetRegressionHistogram()
        {
            var result = (RelatedSample)RegressionSample.Clone();
            for (int i = 0; i < RegressionSample.Count; i++)
            {
                result.Y[i] = InitialSample.Y[i]- RegressionSample.Y[i];
            }

            return result;
        }
        

        #endregion

        #region Arbitratry

        

        public void AddRegressionYForArbitraryX(decimal x,bool needClearingBefore)
        {
            if (RegressionType != RegressionType.Linear)
                throw new ArgumentException("Cannot calculate arbitrary y within a non-linear regression");

            if (needClearingBefore)
            {
                RegressionValues.ClearArbitrary();
                NonLinearRegression.RegressionValues.ClearArbitrary();
            }

            var arbitrary =  GetRegressionYForArbitraryX(x);

           // RegressionValues.Add(arbitrary.normalized);
            //RegressionValues.SortByX();

            NonLinearRegression.RegressionValues.Add(arbitrary.reversed);
            NonLinearRegression.RegressionValues.SortByX();
        }

        #endregion

        #region XML

        public RegressionToXml GetRegressionToXml()
        {
            var result = new RegressionToXml(FullName,GetR2(),GetMMRE(),GetPRED());
            for (var i = 0; i < RegressionValues.Count; i++)
            {
                result.DataItems.Add(new(RegressionValues[i]));
            }

            return result;
        }

        #endregion
    }
}
