using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    public class ProjectsTableRow : TableRow
    {
        public int Id;
        public string Name;
        public decimal Duration;
        public decimal LaborCost;
        public decimal Completeness;
        public ProjectsTableRow(string name, decimal duration, decimal laborCost, decimal completeness)
        {
            Name = name;
            Duration = duration;
            LaborCost = laborCost;
            Completeness = completeness;
        }

        public ProjectsTableRow()
        {

        }
        
        public override List<string> GetPropertiesInOrder()
        {
            return PropertiesInOrder;
        }

        public static List<string> PropertiesInOrder =>
            new List<string>()
            {
                "Id",
                "Name",
                "Duration",
                "LaborCost",
                "Completeness",
            };
    }
}
