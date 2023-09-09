using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class ProjectsTable:Table<ProjectsTableRow>
    {
        public ProjectsTable()
        {
        }

        public int GetId(string projectName)
        {
            foreach (var row in _rows)
            {
                if (String.CompareOrdinal(projectName, row.Name) == 0)
                    return row.Id;
            }

            throw new ArgumentException("project does not exists");
        }
    }
}
