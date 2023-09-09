using System;
using System.Collections;
using System.Collections.Generic;

namespace RegressionBuilder
{
    class HistogramEnum:IEnumerator<HistogramItem>
    {
        private readonly List<HistogramItem> _columns;
        private int _curIndex = -1;

        public HistogramEnum(List<HistogramItem> l)
        {
            _columns = new List<HistogramItem>(l);
        }
        public void Dispose()
        {
            _curIndex = -1;
        }

        public bool MoveNext()
        {
            _curIndex++;
            return (_curIndex < _columns.Count);
        }

        public void Reset()
        {
            _curIndex = -1;
        }

        public HistogramItem Current
        {
            get
            {
                try
                {
                    return _columns[_curIndex];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;
    }
}
