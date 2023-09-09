using System;
using System.Collections.Generic;
using System.Xml;

namespace RegressionBuilder
{
    class TabularItemsTable
    {
        private readonly List<TabularItem> _tabularItems;

        public TabularItemsTable(string path)
        {
            _tabularItems = new List<TabularItem>();
            TryParseDocument(path);
        }
        
        /// <exception cref="ArgumentException"></exception>
        public decimal GetValueFromTable(decimal alpha, int degreesOfFreedomNumber)
        {
            foreach (var item in _tabularItems)
            {
                if (item.DegreesOfFreedomNumber == degreesOfFreedomNumber && item.Alpha == alpha)
                {
                    return item.Val;
                }
            }

            throw new ArgumentException(
                $"There is no appropriate value for alpha={alpha} and degreesOfFreedomNumber={degreesOfFreedomNumber}");
        }

        public List<decimal> GetAllAlpha(int degreesOfFreedomNumber)
        {
            var result = new List<decimal>();
            foreach (var item in _tabularItems)
            {
                if (item.DegreesOfFreedomNumber == degreesOfFreedomNumber)
                {
                    result.Add(item.Alpha);
                }
            }
            
            if(result.Count == 0)
                throw new ArgumentException(
                    $"There is no appropriate values for  degreesOfFreedomNumber={degreesOfFreedomNumber}");

            return result;
        }

        public bool TryParseDocument(string path)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            var xmlRoot = xmlDoc.GetElementsByTagName("columns");
            
            foreach (XmlNode alpha in xmlRoot[0]!)
            {
                foreach (XmlNode row in alpha)
                {
                    try
                    {
                        var ci = new TabularItem();
                        decimal.TryParse(alpha.Attributes?["val"]!.Value, out ci.Alpha);
                        int.TryParse(row.Attributes?["degreesOfFreedomNumber"]!.Value, out ci.DegreesOfFreedomNumber);
                        decimal.TryParse(row.Attributes?["val"]!.Value, out ci.Val);

                        _tabularItems.Add(ci);
                    }
                    catch (NullReferenceException)
                    {
                        return false;
                    }
                   
                }
            }

            return true;
        }
    }
}
