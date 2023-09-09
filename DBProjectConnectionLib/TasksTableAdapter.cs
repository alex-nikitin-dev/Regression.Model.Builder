using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class TasksTableAdapter : TableAdapter<TasksTableRow>
    {
        public TasksTableAdapter(SqlConnection connection) : 
            base(connection, "Tasks")
        {
        }
    }
}
