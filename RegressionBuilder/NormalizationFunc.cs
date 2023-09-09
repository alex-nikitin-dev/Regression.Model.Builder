using System;

namespace RegressionBuilder
{
    class NormalizationFunc
    {
        public Func<double, double> Func { get; }
        public Func<double, double> InverseFunc { get; }
        public SampleNormalizationType Type { get; }

        public NormalizationFunc(Func<double, double> func, Func<double, double> inverseFunc, SampleNormalizationType type)
        {
            Func = func;
            InverseFunc = inverseFunc;
            Type = type;
        }
    }
}
