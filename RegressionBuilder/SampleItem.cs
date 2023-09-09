using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionBuilder
{
    public class SampleData
    {
        private readonly Dictionary<int, decimal> _data;
        private int _idCounter;

        public SampleData()
        {
            _data = new();
            _idCounter = 0;
        }

        public void Add(decimal value)
        {
            _data.Add(_idCounter++,value);
        }

        public void RemoveAt(int index)
        {
        }
    }
}
