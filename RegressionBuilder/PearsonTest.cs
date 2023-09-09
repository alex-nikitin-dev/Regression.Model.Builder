using System;

namespace RegressionBuilder
{
    public class PearsonTest
    {
        private readonly Histogram _histogram;
        private readonly decimal _sampleMean;
        private readonly decimal _standardDeviation;
        private readonly decimal _sampleCount;

        public decimal ChiSquaredStatistic { get; private set; }
        public decimal ChiCritical { get; private set; }
        public bool IsSampleNormal { get; private set; }
        public decimal Alpha { get; private set; }

        public PearsonTest(Sample sample,decimal alpha = 0.05M)
        {
            _histogram = sample.GetHistogram();
            _sampleMean = sample.GetMean();
            _standardDeviation = (decimal)sample.GetStandardDeviation();
            _sampleCount = sample.Count;
            TestSampleForNormal(alpha);
        }

     
        private decimal CalculateChiSquaredStatistic()
        {
            decimal chiSquared = 0;
            foreach (var hi in _histogram)
            {
                var ui = hi.Middle - _sampleMean;
                var ti = ui / _standardDeviation;
                var fti = ProbabilityDensity(ti);
                var ni = (_sampleCount * _histogram.Delta / _standardDeviation) * fti;
                chiSquared += (hi.Height - ni) * (hi.Height - ni) / ni;
            }

            ChiSquaredStatistic = chiSquared;
            return chiSquared;
        }

        private bool TestSampleForNormal(decimal alpha=0.05M)
        {
            Alpha = alpha;
            var chiStatistic = CalculateChiSquaredStatistic();
            var chiCriticalObj = ChiCriticalTable.GetChiCriticalTable();
            var chiCriticalVal = chiCriticalObj.GetValueFromTable(alpha, _histogram.ColumnCount - 3);
            ChiCritical = chiCriticalVal;
            IsSampleNormal = chiStatistic < chiCriticalVal;
            return IsSampleNormal;
        }
    }
}
