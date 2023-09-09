using System;
using System.Threading.Tasks;

namespace RegressionBuilder
{
    public class JohnsonNormalizer
    {
        public Sample TheSample { get; }
        private readonly JohnsonParameters _parameters;

        public JohnsonParameters Parameters => _parameters.Clone();
#if XMAS
        public JohnsonFamily Family { get; set; }
#else
       public JohnsonFamily Family { get; private set; }
#endif
        

        public decimal Phi => _parameters.Phi.Value;
        public decimal Gamma => _parameters.Gamma.Value;
        public decimal Lambda => _parameters.Lambda.Value;
        public decimal Eta => _parameters.Eta.Value;

        private JohnsonNormalizer(Sample theSample, JohnsonFamily family,JohnsonParameters jParams = null)
        :this(theSample, jParams)
        {
            Family = family;
        }

        /// <summary>
        /// JohnsonNormalizer
        /// </summary>
        /// <param name="theSample">The sample which will be changed during normalization. </param>
        /// <param name="jParams">Initial parameters</param>
        public JohnsonNormalizer(Sample theSample, JohnsonParameters jParams = null)
        {
            TheSample = theSample;
            _parameters = jParams != null ? jParams.Clone() : new JohnsonParameters(TheSample.Min, TheSample.Max);
        }

        public void Normalize()
        {
            ChooseFamily();
            Normalize(_parameters, TheSample);
        }

        public async Task NormalizeAsync()
        {
            await Task.Run(Normalize);
        }

        private void Normalize(JohnsonParameters jParams, Sample sample)
        {
            switch (Family)
            {
                case JohnsonFamily.Su:
                    throw new NotImplementedException();
                case JohnsonFamily.Sb:
                    SbNormalization(jParams,sample);
                    break;
                case JohnsonFamily.Sl:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private double GetReversedValueForSBFamily(decimal value)
        {
            var t = (value - Gamma) / Eta;
            var et = Math.Pow(Math.E, (double)t);

            return (et*((double)Lambda + (double)Phi) + (double)Phi)/( 1.0 + et);
        }
        private double GetNormalizedValueForSBFamily(decimal value)
        {
            return (double)(Gamma + Eta * (decimal)Math.Log((double)((value - Phi) / (Lambda + Phi - value))));
        }
        private void ReverseSBNormalization()
        {
            for (int i = 0; i < TheSample.Count; i++)
            {
                TheSample[i] = (decimal)GetReversedValueForSBFamily(TheSample[i]);
            }
        }

        public decimal GetReversedValue(decimal value)
        {
            if (TheSample.NormalizationType != SampleNormalizationType.Johnson)
                throw new InvalidOperationException(
                    "Cannot reverse the single value: the normalization type is not Johnson");

            switch (Family)
            {
                case JohnsonFamily.Su:
                    throw new NotImplementedException();
                case JohnsonFamily.Sb:
                    return (decimal)GetReversedValueForSBFamily(value);
                case JohnsonFamily.Sl:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public decimal GetNormalizedValue(decimal value)
        {
            if (TheSample.NormalizationType != SampleNormalizationType.Johnson)
                throw new InvalidOperationException(
                    "Cannot normalize the single value: the normalization type is not Johnson");

            switch (Family)
            {
                case JohnsonFamily.Su:
                    throw new NotImplementedException();
                case JohnsonFamily.Sb:
                    return (decimal)GetNormalizedValueForSBFamily(value);
                case JohnsonFamily.Sl:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reverse()
        {
            if (TheSample.NormalizationType != SampleNormalizationType.Johnson)
                throw new InvalidOperationException(
                    "Cannot reverse Johnson normalization the sample has not been normalized this way");

            switch (Family)
            {
                case JohnsonFamily.Su:
                    throw new NotImplementedException();
                case JohnsonFamily.Sb:
                    ReverseSBNormalization();
                    break;
                case JohnsonFamily.Sl:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SbNormalization(JohnsonParameters jParams,Sample sample)
        {
            for (var i = 0; i < sample.Count; i++)
            {
                sample[i] = jParams.Gamma.Value + jParams.Eta.Value * (decimal)Math.Log((double)((TheSample[i] - jParams.Phi.Value)/
                    (jParams.Lambda.Value + jParams.Phi.Value - TheSample[i])));
            }
        }

        private double GetUpperBoundOfCriticalRegion()
        {
            var skewness = TheSample.GetSkewness();
            return skewness * skewness + 1;
        }

       

        private void ChooseFamily()
        {
            // ReSharper disable once InconsistentNaming
            var E = (decimal)GetUpperBoundOfCriticalRegion();
            // ReSharper disable once InconsistentNaming
            var SL = (decimal)GetSl();
            var kurtosis = (decimal)TheSample.GetKurtosis();
            if (kurtosis > SL)
            {
                Family = JohnsonFamily.Su;
            }
            else if (kurtosis == SL)
            {
                Family = JohnsonFamily.Sl;
            }
            else if (kurtosis > E && kurtosis < SL)
            {
                Family = JohnsonFamily.Sb;
            }
            else
            {
                throw new ArgumentException("The kurtosis is in the critical region");
            }
        }

        public JohnsonNormalizer Clone(Sample newInstance)
        {
            return new JohnsonNormalizer(newInstance, Family, _parameters.Clone());
        }

    }
}
