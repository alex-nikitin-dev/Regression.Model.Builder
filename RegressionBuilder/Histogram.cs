using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RegressionBuilder
{
    public class Histogram: IEnumerable<HistogramItem>
    {
        private readonly List<HistogramItem> _columns;
        public  int ColumnCount => _columns.Count;

        public decimal Delta { get; private set; }

        public HistogramItem this[int index] => _columns[index];


        public Histogram(List<decimal> rawData)
        {
            _columns = new List<HistogramItem>();
            var numberOfEmptyColumns = 0;
            do
            {
                GenerateColumns(rawData.Count, rawData.Min(), rawData.Max(), numberOfEmptyColumns);
                FillColumns(rawData);
                numberOfEmptyColumns = GetNumberOfEmptyColumns();
            } while (numberOfEmptyColumns != 0);
        }

        private void FillColumns(List<decimal> rawData)
        {
            foreach (var randomVar in rawData)
            {
                foreach (var column in _columns)
                {
                    if (randomVar >= column.Start && randomVar <= column.End)
                    {
                        column.RandomVars.Add(randomVar);
                        break;
                    }
                }
            }
        }

        private int GetNumberOfEmptyColumns()
        {
            var result = 0;
            foreach (var column in _columns)
            {
                if (column.RandomVars.Count == 0)
                    result++;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="countOfData"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="reduceBy">How many columns need to reduce</param>
        private void GenerateColumns(int countOfData, decimal min, decimal max, int reduceBy =0)
        {

#if XMAS || GEN5COLS
            var countColumns = 5;
#else
           var countColumns = CalculateCountColumns(countOfData, reduceBy);
#endif

            
            var start = min;
            var delta = (max - min) / countColumns;
            _columns.Clear();
            for (decimal i = 0; i < countColumns; i++)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                var hi = new HistogramItem();
                hi.Start = start + delta * i;
                hi.End = (i == countColumns - 1) ? max : start + delta * (i + 1);
                hi.Middle = (hi.Start + hi.End) / 2;
                _columns.Add(hi);
            }

            Delta = delta;
        }

        private static decimal CalculateCountColumns(int countOfRawData, int reduceBy = 0)
        {
            var count= (3.3 * Math.Log10(countOfRawData) + 1);
            var roundCount = RoundCountColumns((decimal)count);
            if (reduceBy > 0 && reduceBy % 2 != 0)
                reduceBy++;
            if (roundCount < reduceBy + 3)
                throw new ArgumentException($"Cannot calculate histograms columns: countColumns {roundCount} < reduceBy {reduceBy} + 3");
            return roundCount - reduceBy;
        }

        /// <summary>
        /// Round to the nearest odd number above
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static decimal RoundCountColumns(decimal count)
        {
            var round = decimal.Round(count);
            for (decimal i = 0; round % 2 == decimal.Zero; round += ++i)
            {
            }

            return round;
        }

        public IEnumerator<HistogramItem> GetEnumerator()
        {
            return new HistogramEnum(_columns);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
