using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class TypeTaskTable:Table<TypeTaskTableRow>
    {
        public TypeTaskTable()
        {

        }

        public int GetId(string name)
        {
            foreach (var row in _rows)
            {
                if (String.CompareOrdinal(row.Name, name) == 0)
                {
                    return row.Id;
                }
            }

            throw new ArgumentException("row does not exists");
        }
    }
}
