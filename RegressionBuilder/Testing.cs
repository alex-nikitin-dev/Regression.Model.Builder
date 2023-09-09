using System.Collections;
using System.Collections.Generic;

namespace RegressionBuilder
{
    class Testing: IEnumerable<TestItem>
    {
        private readonly List<TestItem> _a;
       
        public Testing()
        {
            _a  = new List<TestItem>();
            for (int i = 0; i < 10; i++)
            {
                _a.Add(new TestItem(i, i+1));
            }
        }

        public IEnumerator<TestItem> GetEnumerator()
        {
            return new TestingEnum(_a);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
