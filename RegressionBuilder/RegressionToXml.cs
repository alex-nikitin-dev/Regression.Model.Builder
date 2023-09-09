using System.Collections.Generic;

namespace RegressionBuilder
{
    public class RegressionToXml
    {
        public string FullName;
        public decimal R2;
        public decimal MMRE;
        public decimal PRED;
        public List<RegressionListItem> DataItems;

        public RegressionToXml()
        {
            DataItems = new List<RegressionListItem>();
        }

        public RegressionToXml(string fullName, decimal r2, decimal mmre, decimal pred)
        : this()
        {
            FullName = fullName;
            R2 = r2;
            MMRE = mmre;
            PRED = pred;
        }
    }
}
