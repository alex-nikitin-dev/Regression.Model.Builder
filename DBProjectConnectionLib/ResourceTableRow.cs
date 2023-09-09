using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    public class ResourceTableRow:TableRow
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public int PositionId;

        public ResourceTableRow(string firstName, string lastName, int positionId)
        {
            FirstName = firstName;
            LastName = lastName;
            PositionId = positionId;
        }

        public ResourceTableRow()
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
                "FirstName",
                "LastName",
                "PositionId"
            };
    }
}
