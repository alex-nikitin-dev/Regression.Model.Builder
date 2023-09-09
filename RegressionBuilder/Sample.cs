using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegressionBuilder
{
    public class Sample:
        IEnumerable<decimal>,
        ICloneable
    {
        #region .Properties
        private readonly List<decimal> _randomValues;

        public string Name { get; set; }

        public string DependentName => $"{Name} ({(Dependent ? "Y" : "X")})";

        public string FullName => $"{Name} {(Dependent ? "Y" : "X")} ({NormalizationType.GetDisplayName()})";

#if XMAS
        public SampleNormalizationType NormalizationType { get;  set; }
        public JohnsonNormalizer _johnsonNormalizer;
#else
        public SampleNormalizationType NormalizationType { get; private set; }
        private JohnsonNormalizer _johnsonNormalizer;
#endif
       
        public JohnsonParameters JohnsonParameters => _johnsonNormalizer?.Parameters;

        

        /// <summary>
        /// Always Recalculates
        /// </summary>
        public decimal Min => _randomValues.Min();
        /// <summary>
        /// Always Recalculates
        /// </summary>
        public decimal Max => _randomValues.Max();

        public int Count => _randomValues.Count;

        public bool Dependent;
        

        #endregion
        
        #region .ctor
        private Sample(string name, bool dependent,
            SampleNormalizationType normType,
            JohnsonNormalizer jNormalizer)
        {
            Name = name;
            Dependent = dependent;
            _randomValues = new List<decimal>();
            NormalizationType = normType;
            _johnsonNormalizer = jNormalizer;
        }
        //private Sample(List<decimal> randomValues,string name,bool dependent,
        //    SampleNormalizationType normType,
        //    JohnsonNormalizer jNormalizer):
        //    this(name,dependent,normType,jNormalizer)
        //{
        //    Set(randomValues);
        //} 

        public Sample(string name, bool dependent)
            : this(name, dependent, SampleNormalizationType.None, null)
        {

        }

        public Sample(List<decimal> randomValues,string name,bool dependent):
            this(name,dependent)
        {
            Set(randomValues);
        }

        #endregion

        #region .Enumerator

        public decimal this[int index]
        {
            get => _randomValues[index];
            set => _randomValues[index] = value;
        }
        public IEnumerator<decimal> GetEnumerator()
        {
            return new SampleEnum(_randomValues);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get copy
        /// </summary>
        /// <returns></returns>
        public decimal[] GetArray()
        {
            return _randomValues.ToArray();
        }

        public List<decimal> GetCopyRandomValues()
        {
            return new List<decimal>(GetArray());
        }

        #endregion

        #region Values Operations Add, Set, Remove
        public void Set(List<decimal> randomValues)
        {
            _randomValues.Clear();
            _randomValues.AddRange(randomValues);
        }
       
        public void Add(decimal randomValue)
        {
            _randomValues.Add(randomValue);
        }

        public void AddRange(List<decimal> randomValues)
        {
            _randomValues.AddRange(randomValues);
        }

        public void RemoveAt(int index)
        {
            _randomValues.RemoveAt(index);
        }
        public Histogram GetHistogram()
        {
            return new Histogram(_randomValues);
        }
        #endregion

        #region Main calculations

        #region Additional functions. Old ones.
        /// <summary>
        /// ByHistogram
        /// </summary>
        public double GetCentralMoment1(uint n)
        {
            var histogram = GetHistogram();
            var sampleMean = GetMean();
            double centralMoment = 0;
            foreach (var hi in histogram)
            {
                centralMoment += MathHelper.Exp((double)(hi.Middle - sampleMean), n) * (double)hi.Height;
            }

            return centralMoment / (Count - 1);
        }
        public double GetKurtosis1()
        {
            return GetCentralMoment(4) / MathHelper.Exp(GetStandardDeviation(), 4);
        }
        public decimal GetMeanByHistogram()
        {
            decimal mean = 0;
            var histogram = GetHistogram();
            foreach (var item in histogram)
            {
                mean += item.Middle * item.Height;
            }

            return mean / Count;
        }
        #endregion

        public double GetCentralMoment(uint n)
        {
            return GetSumDiffXAndSampleMean(GetMean(),n) / (Count - 1);
        }

        public double GetVariance()
        {
            return GetCentralMoment(2);
        }

        public double GetStandardDeviation()
        {
            return  Math.Sqrt(GetVariance());
        }

        public double GetSkewness()
        {
            return GetCentralMoment(3) / MathHelper.Exp(GetStandardDeviation(), 3);
        }

        public double GetKurtosis()
        {
            var k1 = (double)(Count * Count - 2 * Count + 3);
            var mean = GetMean();
            var sum2 = GetSumDiffXAndSampleMean(mean, 2);
            var sum4 = GetSumDiffXAndSampleMean(mean, 4);
            var k2 = (2 * Count - 3) * 3 / (double)Count;
            var s2 =  sum2 / (Count - 1);

            var result =  (k1 * sum4 + k2 * sum2 * sum2) / 
                          ((Count - 1) * (Count - 2) * (Count - 3) * s2 * s2);
            return result;
        }

        public decimal GetMean()
        {
            return _randomValues.Sum() / Count;
        }

        public double GetSumSquaredDiffXAndSampleMean()
        {
            return GetSumDiffXAndSampleMean(GetMean(), 2);
        }

        private double GetSumDiffXAndSampleMean(decimal sampleMean, uint power)
        {
            return _randomValues.Sum(item => MathHelper.Exp((double)(item - sampleMean), power));
        }
        public decimal Sum()
        {
            return _randomValues.Sum();
        }

        public double SumPowered(uint power)
        {
            return _randomValues.Sum(item => MathHelper.Exp((double)item, power));
        }

        #endregion

        #region Normalization
        public bool IsNormal()
        {
            return new PearsonTest(this).IsSampleNormal;
        }

        public PearsonTest GetPearsonTest()
        {
            return new PearsonTest(this);
        }
        /// <summary>
        /// Get logarithm copy
        /// </summary>
        public Sample GetNormalized(Func<double,double> log, SampleNormalizationType type)
        {
            var clone = (Sample)Clone();
            clone.Normalize(log,type);
            return clone;
        }

        public void Normalize(Func<double, double> log, SampleNormalizationType type)
        {
            for (int i = 0; i < Count; i++)
            {
                _randomValues[i] = (decimal)log((double)_randomValues[i]);
            }
            NormalizationType  = type;
        }

        /// <summary>
        /// JohnsonNormalizeAsync
        /// </summary>
        /// <param name="jParams">Initial parameters for Johnson normalization</param>
        /// <returns></returns>
        public async Task JohnsonNormalizeAsync(JohnsonParameters jParams = null)
        {
            _johnsonNormalizer = new JohnsonNormalizer(this, jParams);
            await _johnsonNormalizer.NormalizeAsync();
            NormalizationType = SampleNormalizationType.Johnson;
        }

        public void ReverseNormalization()
        {
            switch (NormalizationType)
            {
                case SampleNormalizationType.None:
                    throw new InvalidOperationException(
                        "Cannot reverse normalization: the data has not been normalized");
                case SampleNormalizationType.Johnson:
                    ReverseJohnsonNormalization();
                    break;
                case SampleNormalizationType.Log10:
                    Normalize(x =>  Math.Pow(10, x),SampleNormalizationType.None);
                    break;
                case SampleNormalizationType.Log:
                    Normalize(x =>  Math.Pow(Math.E, x),SampleNormalizationType.None);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            NormalizationType = SampleNormalizationType.None;
        }

       

        private void ReverseJohnsonNormalization()
        {
            if (_johnsonNormalizer == null)
                throw new ArgumentException("Cannot reverse Johnson normalization: the normalizer object does not exist");

            _johnsonNormalizer.Reverse();
        }

        public decimal ReverseValue(decimal value)
        {
            return NormalizationType switch
            {
                SampleNormalizationType.None => throw new InvalidOperationException(
                    "Cannot reverse normalization of the single value: Sample normalization type is None"),
                SampleNormalizationType.Johnson => GetJohnsonReversedValue(value),
                SampleNormalizationType.Log10 => new Func<decimal, decimal>(x =>  (decimal)Math.Pow(10, (double)x))(value),
                SampleNormalizationType.Log => new Func<decimal, decimal>(x =>  (decimal)Math.Pow(Math.E, (double)x))(value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public decimal GetNormalizedValue(decimal value)
        {
            return NormalizationType switch
            {
                SampleNormalizationType.None => throw new InvalidOperationException(
                    "Cannot reverse normalization of the single value: Sample normalization type is None"),
                SampleNormalizationType.Johnson => GetJohnsonNormalizedValue(value),
                SampleNormalizationType.Log10 => new Func<decimal, decimal>(x =>  (decimal)Math.Log10((double)x))(value),
                SampleNormalizationType.Log => new Func<decimal, decimal>(x =>  (decimal)Math.Log((double)x))(value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private decimal GetJohnsonNormalizedValue(decimal value)
        {
            return _johnsonNormalizer.GetNormalizedValue(value);
        }

        private decimal GetJohnsonReversedValue(decimal value)
        {
            return _johnsonNormalizer.GetReversedValue(value);
        }

        #endregion

        #region .Clone Copy

        public object Clone()
        {
            var result = CloneEmpty();           
            for (int i = 0; i < _randomValues.Count; i++)
            {
                result.Add(_randomValues[i]);
            }

            return result;
        }

        /// <summary>
        /// Clone the sample (all parameters) without cloning the data (values).
        /// List of data will be created.
        /// </summary>
        /// <returns>new object</returns>
        public Sample CloneEmpty()
        {
            var result =  new Sample(Name, Dependent, NormalizationType, null); 
            result._johnsonNormalizer = _johnsonNormalizer?.Clone(result);
            return result;
        }

        public void Clear()
        {
            _randomValues.Clear();
            NormalizationType = SampleNormalizationType.None;
        }

        public void CopyValues(Sample sample)
        {
            if (sample.Count != Count) 
                throw new ArgumentException("Samples count is not equal!");

            for (int i = 0; i < _randomValues.Count; i++)
            {
                _randomValues[i] = sample[i];
            }
        }

        #endregion

        #region To string. Get Info
        public List<(string name, string val)> GetSampleProperties(int precision)
        {
            var pearsonTest = GetPearsonTest();
            var result = new List<(string name, string val)>
            {
                ("Count of data", Count.ToString()),
                ("Variance", $"{Math.Round(GetVariance(), precision)}"),
                ("Standard Deviation", $"{Math.Round(GetStandardDeviation(), precision)}"),
                ("Skewness", $"{Math.Round(GetSkewness(), precision)}"),
                ("Kurtosis", $"{Math.Round(GetKurtosis(), precision)}"),
                ("Mean", $"{Math.Round(GetMean(), precision):G29}"),
                ("Minimum", $"{Math.Round(Min, precision):G29}"),
                ("Maximum", $"{Math.Round(Max, precision):G29}"),
                ("Person's Test", pearsonTest.IsSampleNormal ? "Normal" : "Not Normal"),
                ("Observed Chi", $"{Math.Round(pearsonTest.ChiSquaredStatistic , precision):G29}"),
                ("Critical Chi", $"{Math.Round(pearsonTest.ChiCritical , precision):G29}"),
                ("Alpha", $"{Math.Round(pearsonTest.Alpha , precision):G29}")
            };

            if (NormalizationType == SampleNormalizationType.Johnson)
                result.AddRange(_johnsonNormalizer.Parameters.GetListParameters(precision));

            return result;
        }
        

        #endregion
    }
}
