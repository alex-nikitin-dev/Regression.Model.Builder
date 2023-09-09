using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class TypeTaskTableRow : TableRow
    {
        public int Id;
        public string Name;
        public string Description;

        public TypeTaskTableRow(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public TypeTaskTableRow()
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
                "Description"
            };


    }
}
