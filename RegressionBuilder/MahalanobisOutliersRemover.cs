using System;
using System.Collections.Generic;
using LinearAlgebra;

namespace RegressionBuilder
{
    public class MahalanobisOutliersRemover
    {
        private readonly RelatedSample _normalizedSample;
        private readonly RelatedSample _sample;
        private readonly decimal _critical;
        public MahalanobisOutliersRemover(RelatedSample normalizedSample, RelatedSample sample,  decimal critical)
        {
            if (normalizedSample.Count != sample.Count) 
                throw new ArgumentException("Cannot proceed MahalanobisOutliersRemover creation. Samples counts are not equal. ");

            _normalizedSample = normalizedSample;
            _sample = sample;
            _critical = critical;
        }

        public (RelatedSample sample, RelatedSample normalized, List<int> indexes) FindOutliers()
        {
            var distancesMatrix = GetDistancesMatrix();
            var indexes = new List<int>();
            var result = _sample.CloneEmpty();
            var normResult = _normalizedSample.CloneEmpty();

            for (int i = 0; i < distancesMatrix.CountRows; i++)
            {
                if (distancesMatrix[i, i] <= _critical)
                {
                    result.Add(_sample.X[i], _sample.Y[i]);
                    normResult.Add(_normalizedSample.X[i], _normalizedSample.Y[i]);
                }
                else
                    indexes.Add(i);
            }

            return (result, normResult, indexes);
        }
    }
}
