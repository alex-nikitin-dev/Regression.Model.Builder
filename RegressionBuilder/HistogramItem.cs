using System.Collections.Generic;

namespace RegressionBuilder
{
    public class HistogramItem
    {
        public decimal Start;
        public decimal End;
        public decimal Middle;
        public decimal Height => RandomVars.Count;
        public List<decimal> RandomVars = new();
    }
}
