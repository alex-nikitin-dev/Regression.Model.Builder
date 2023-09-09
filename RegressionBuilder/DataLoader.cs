using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using DBProjectConnectionLib;

namespace RegressionBuilder
{
    internal class DataLoader
    {
        public static RelatedSample GetDataFromDataBase(string connectionString)
        {
            var xData = new List<decimal>();
            var yData = new List<decimal>();
            using (var connector = new ProjectsDBConnector(new SqlConnectionStringBuilder(connectionString)))
            {
                connector.Open();
                var projTable = new ProjectsTable();
                new ProjectsTableAdapter(connector.Connection).Fill(projTable);
                foreach (var row in projTable)
                {
                    if (row.LaborCost == decimal.Zero || row.Duration == decimal.Zero) continue;
                    xData.Add(row.LaborCost);
                    yData.Add(row.Duration);
                }
            }

            return new RelatedSample(xData, yData, "Labor Cost, man hours", "Duration Of Work, days");
        }

        public static RelatedSample GetDataFromXml(string path)
        {   
            var xData = new List<decimal>();
            var yData = new List<decimal>();
            var document = XDocument.Load(path);
            foreach (var (x, y) in ReadXml(document))
            {
                xData.Add(x);
                yData.Add(y);
            }
            return new RelatedSample(xData, yData, document.Root!.Attribute("xName")?.Value, document.Root.Attribute("yName")?.Value);
        }

        private static IEnumerable<(decimal x, decimal y)> ReadXml(XDocument document)
        {
            return document
                .Root!
                .Elements()
                .Select(row => (
                    decimal.Parse(row.Element("x")!.Value),
                    decimal.Parse(row.Element("y")!.Value)
                    ));
        }
    }
}
