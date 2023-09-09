using System;
using System.Collections;
using System.Collections.Generic;

namespace RegressionBuilder
{
    class SampleEnum:IEnumerator<decimal>
    {
        private readonly List<decimal> _data;
        private int _curIndex = -1;
        public SampleEnum(List<decimal> data)
        {
            _data = new List<decimal>(data);
        }
        public void Dispose()
        {
            _curIndex = -1;
        }

        public bool MoveNext()
        {
            _curIndex++;
            return (_curIndex < _data.Count);
        }

        public void Reset()
        {
            _curIndex = -1;
        }

        public decimal Current
        {
            get
            {
                try
                {
                    return _data[_curIndex];
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
