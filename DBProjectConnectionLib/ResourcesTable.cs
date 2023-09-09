using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class ResourcesTable:Table<ResourceTableRow>
    {
        public ResourcesTable()
        {

        }

        public int GetId(string firstName, string lastName)
        {
            foreach (var row in _rows)
            {
                if (String.CompareOrdinal(row.FirstName, firstName) == 0 &&
                    String.CompareOrdinal(row.LastName, lastName) == 0)
                {
                    return row.Id;
                }
            }

            throw new ArgumentException("row does not exists");
        }
    }
}
