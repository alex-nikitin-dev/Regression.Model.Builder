using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    public class TasksTableRow:TableRow
    {
        public int Id;
        public string Name;
        public decimal Duration;
        public decimal LaborCost;
        public int ResourceId;
        public int TypeTaskId;
        public int ProjectId;

        public TasksTableRow(string name, decimal duration, decimal laborCost, int resourceId, int typeTaskId, int projectId)
        {
            Name = name;
            Duration = duration;
            LaborCost = laborCost;
            ResourceId = resourceId;
            TypeTaskId = typeTaskId;
            ProjectId = projectId;
        }

        public TasksTableRow()
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
                "ResourceId",
                "TypeTaskId",
                "ProjectId"
            };
    }
}
