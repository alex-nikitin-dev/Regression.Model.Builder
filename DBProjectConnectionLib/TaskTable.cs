using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class TaskTable
    {
        private List<TasksTableRow> _rows;

        public TaskTable(List<TasksTableRow> rows)
        {
            _rows = new List<TasksTableRow>(rows);
        }
    }
}
