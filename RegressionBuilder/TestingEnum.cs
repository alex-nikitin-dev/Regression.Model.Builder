using System;
using System.Collections;
using System.Collections.Generic;

namespace RegressionBuilder
{
    class TestingEnum : IEnumerator<TestItem>
    {
        private readonly List<TestItem> _a;
        private int _curIndex = -1;
        public TestingEnum(List<TestItem> l)
        {
           _a = new List<TestItem>();
           foreach (var testItem in l)
           {
               _a.Add(new TestItem(testItem.A, testItem.B));
               _a[0].A = 1;
           }
        }

        public void Dispose()
        {
            _curIndex = -1;
        }

        public bool MoveNext()
        {
            _curIndex++;
            return (_curIndex < _a.Count);
        }

        public void Reset()
        {
            _curIndex = -1;
        }

        public TestItem Current
        {
            get
            {
                try
                {
                    return _a[_curIndex];
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
