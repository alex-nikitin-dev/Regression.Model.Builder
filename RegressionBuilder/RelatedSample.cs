using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LinearAlgebra;

namespace RegressionBuilder
{
    public class RelatedSample:ICloneable
    {
        private readonly List<Sample> _sampleArray;

        #region .ctor

        private RelatedSample()
        {
            NormalizationType = SampleNormalizationType.None;
        }

        public RelatedSample(string nameX, string nameY)
            : this()
        {
            _sampleArray = new List<Sample> {new(nameX, false), new(nameY, true)};
        }

        public RelatedSample(List<decimal> independentDataX, List<decimal> dependentDataY,string nameX,string nameY)
            : this()
        {
            if(independentDataX.Count != dependentDataY.Count) 
                throw new ArgumentException("independentDataX.Count != dependentDataY.Count");
            _sampleArray = new List<Sample>
            {
                new(independentDataX,nameX, false), 
                new(dependentDataY, nameY,true)
            };
        }

        public RelatedSample(Sample dataX, Sample dataY)
            : this()
        {
            if (dataX.Count != dataY.Count)
                throw new ArgumentException("dataX.Count != dataY.Count");
            _sampleArray = new List<Sample> {dataX, dataY};

            if (dataX.NormalizationType != dataY.NormalizationType)
                throw new ArgumentException("Cannot create RelatedSample: dataX.NormalizationType != dataY.NormalizationType");

            NormalizationType = dataY.NormalizationType;
        }

        #endregion

        #region .Properties
        public Sample X => _sampleArray[0];
        public Sample Y => _sampleArray[1];

        public int CountDimensions => _sampleArray.Count;
        /// <summary>
        /// Number of data in one dimension
        /// </summary>
        public int Count => X.Count;
        public  (decimal X,decimal Y) this[int index] => (X[index], Y[index]);
        
        #if XMAS
        public SampleNormalizationType NormalizationType { get;  set; }
        #else
        public SampleNormalizationType NormalizationType { get; private set; }
        #endif
        #endregion

        #region Add, Set, Remove, Sort, Clone

        public void Clear()
        {
            X.Clear();
            Y.Clear();
        }

        public void Add(decimal x, decimal y)
        {
            X.Add(x);
            Y.Add(y);
        }
        public void RemoveAt(int index)
        {
            foreach (var sample in _sampleArray)
            {
                sample.RemoveAt(index);
            }
        }
        public List<(decimal, decimal)> GetMatrix()
        {
            var list = new List<(decimal,decimal)>();
            for (int i = 0; i < Count; i++)
            {
                list.Add(this[i]);
            }

            return list;
        }

        public void SortByX()
        {
            SortBy(0);
        }
        public enum SortOrder
        {
            ByX = 0,
            ByY = 1
        }

        private void SortBy(SortOrder order)
        {
            var toSort = new List<List<decimal>>
            {
                new(X.GetArray()), 
                new(Y.GetArray())
            };
            ArraySorter.SortBy(toSort, (int) order);
            X.Set(toSort[0]);
            Y.Set(toSort[1]);
        }
        
        public object Clone()
        {
            return new RelatedSample((Sample) X.Clone(), (Sample) Y.Clone());
        }

        /// <summary>
        /// Clone the sample without cloning the data (values)
        /// </summary>
        /// <returns>new object</returns>
        public RelatedSample CloneEmpty()
        {
            return new RelatedSample(X.CloneEmpty(),Y.CloneEmpty());
        }

        #endregion

        #region Normalization

         /// <summary>
        /// Get clone
        /// </summary>
        /// <param name="func"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public RelatedSample GetNormalized(Func<double,double> func, SampleNormalizationType type)
        {
            var clone = (RelatedSample)Clone();
            clone.Normalize(func,type);
            return clone;
        }
         public void Normalize(Func<double, double> func, SampleNormalizationType type)
        {
            foreach (var sample in _sampleArray)
            {
               sample.Normalize(func,type);
            }

            NormalizationType = type;
        }

        public async Task JohnsonNormalize(JohnsonParameters jParamsX = null, JohnsonParameters jParamsY = null)
        {
            var xTask = X.JohnsonNormalizeAsync(jParamsX);
            var yTask = Y.JohnsonNormalizeAsync(jParamsY);

            await xTask;
            await yTask;

            NormalizationType = SampleNormalizationType.Johnson;
        }

        public void ReverseNormalization()
        {
            X.ReverseNormalization();
            Y.ReverseNormalization();
            NormalizationType = SampleNormalizationType.None;
        }


        public delegate void JohnsonNormalizationStepDelegate(RelatedSample normalized, string info);

        public event JohnsonNormalizationStepDelegate JohnsonNormalizationStep;

        public void OnJohnsonNormalizationStep(RelatedSample normalized, string info)
        {
            JohnsonNormalizationStep?.Invoke(normalized, info);
        }

        public async Task<(RelatedSample sample, RelatedSample normalized)> GetJohnsonNormalizedWithOutliers(
            JohnsonParameters jParamsX = null,
            JohnsonParameters jParamsY = null,
            decimal alphaForMahalanobis = 0.005M)
        {
            var chiCritical = ChiCriticalTable
                .GetChiCriticalTable()
                .GetValueFromTable(alphaForMahalanobis, CountDimensions);

            var sampleToNormalize = (RelatedSample) Clone();
            var original = (RelatedSample) Clone();

            for (int step=0, outliers =0;;step++)
            {
                OnJohnsonNormalizationStep(null, $"New iteration started. Iteration: {step} Outliers: {outliers} ...");
                await sampleToNormalize.JohnsonNormalize(jParamsX, jParamsY);
                OnJohnsonNormalizationStep(sampleToNormalize, $"Normalized. Iteration: {step} Outliers: {outliers} remove outliers...");

                var remover = new MahalanobisOutliersRemover(sampleToNormalize, original, chiCritical);
                var result = remover.FindOutliers();

                if (result.indexes.Count != 0)
                {
                    outliers += result.indexes.Count;
                    OnJohnsonNormalizationStep(null, $"Outliers removed. Iteration: {step} Outliers: {outliers}");
                    jParamsX = sampleToNormalize.X.JohnsonParameters.Clone();
                    jParamsY = sampleToNormalize.Y.JohnsonParameters.Clone();

                    original = (RelatedSample) result.sample.Clone();
                    sampleToNormalize = result.sample;

                    var x = sampleToNormalize.X;
                    var y = sampleToNormalize.Y;
                    jParamsX.Balance(x.Min, x.Max);
                    jParamsY.Balance(y.Min, y.Max);
                }
                else
                {
                    OnJohnsonNormalizationStep(null, $"Finished. Iteration: {step} Outliers: {outliers}");
                    return (result.sample, result.normalized);
                }
            }
        }
        public bool IsNormal()
        {
            return X.IsNormal() && Y.IsNormal();
        }
        #endregion

        #region Calculations

        public decimal GetSumSquaredDiffXYAndSampleMean()
        {
            decimal sum = 0;
            var xMean = X.GetMean();
            var yMean = Y.GetMean();
            for (int i = 0; i < Count; i++)
            {
                sum += (X[i] - xMean) * (Y[i] - yMean);
            }
            return sum;
        }

        public Matrix GetCovarianceMatrix()
        {
            var xSum = (decimal)X.GetSumSquaredDiffXAndSampleMean() / Count;
            var ySum = (decimal)Y.GetSumSquaredDiffXAndSampleMean() / Count;
            var xySum = GetSumSquaredDiffXYAndSampleMean() / Count;

            var result = new Matrix("", 2, 2)
            {
                [0, 0] = xSum,  [0, 1] = xySum,
                [1, 0] = xySum, [1, 1] = ySum
            };

            return result;
        }


        #endregion

        #region To string
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public string ToString(int precision)
        {
            var result = "";
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < CountDimensions; j++)
                {
                    result += $@"{Math.Round(_sampleArray[j][i], precision):G29} ";
                }

                result += Environment.NewLine;
            }

            return result;
        }
        #endregion
    }
}
