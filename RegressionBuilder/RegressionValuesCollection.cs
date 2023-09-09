using System;
using System.Collections;
using System.Collections.Generic;

namespace RegressionBuilder
{
    public class RegressionValuesCollection
        :IEnumerable<RegressionItem>,
            ICloneable
    {

        private readonly List<RegressionItem> _items;

        public RegressionValuesCollection()
        {
            _items = new List<RegressionItem>();
        }

        public void Add(RegressionItem item)
        {
            _items.Add(item);
        }

        public int Count => _items.Count;

        public object Clone()
        {
            var result = new RegressionValuesCollection();
            for (int i = 0; i < _items.Count; i++)
            {
                result.Add((RegressionItem)_items[i].Clone());
            }

            return result;
        }

        public void ClearAll()
        {
            _items.Clear();
        }

        public void ClearArbitrary()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Arbitrary)
                {
                    _items.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ClearMainItems()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (!_items[i].Arbitrary)
                {
                    _items.RemoveAt(i);
                    i--;
                }
            }
        }

        public RegressionItem this[int index] => _items[index];

        public IEnumerator<RegressionItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void SortByX()
        {
            var arr = _items.ToArray();
            Array.Sort(arr, (x, y) => Comparer<decimal>.Default.Compare(x.RegressionPoint.X, y.RegressionPoint.X));
            ArraySorter.FillArray(_items, arr);
        }

        public List<RInterval> GetPredictionIntervals()
        {
            var result = new List<RInterval>();
            for (int i = 0; i < _items.Count; i++)
            {
                result.Add(_items[i].PI);
            }

            return result;
        }
        public List<RInterval> GetConfidenceIntervals()
        {
            var result = new List<RInterval>();
            for (int i = 0; i < _items.Count; i++)
            {
                result.Add(_items[i].CI);
            }

            return result;
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public RegressionValuesCollection GetArbitraryValues()
        {
            return GetValues(true);
        }

        private RegressionValuesCollection GetValues(bool arbitrary)
        {
            var result = new RegressionValuesCollection();
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Arbitrary == arbitrary)
                {
                    result.Add(_items[i]);
                }
            }

            return result;
        }
        public RegressionValuesCollection GetMainValues()
        {
            return GetValues(false);
        }
    }
}
