using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    public class PositionTableRow : TableRow
    {
        public int Id;
        public string Name;

        public override List<string> GetPropertiesInOrder()
        {
            return PropertiesInOrder;
        }

        public PositionTableRow(string name)
        {
            Name = name;
        }

        public PositionTableRow()
        {
           
        }

        public  static List<string> PropertiesInOrder =>
            new List<string>()
            {
                "Id",
                "Name"
            };
    }
}
