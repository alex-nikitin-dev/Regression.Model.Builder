using System;
using System.Collections;
using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    class TableEnum<TRow>:IEnumerator<TRow> where TRow:TableRow
    {
        private readonly List<TRow> _data;
        private int _curIndex = -1;
        public TableEnum(List<TRow> data)
        {
            _data = new List<TRow>(data);
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

        public TRow Current
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
