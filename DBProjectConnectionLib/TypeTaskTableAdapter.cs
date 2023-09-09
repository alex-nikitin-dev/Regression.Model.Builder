using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public class TypeTaskTableAdapter : TableAdapter<TypeTaskTableRow>
    {
        public TypeTaskTableAdapter(SqlConnection connection) :
            base(connection, "TypeTask")
        {

        }
    }
}
