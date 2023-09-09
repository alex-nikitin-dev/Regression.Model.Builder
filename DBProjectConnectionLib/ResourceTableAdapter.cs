using System.Data;
using System.Data.SqlClient;

namespace DBProjectConnectionLib
{
    public class ResourceTableAdapter : TableAdapter<ResourceTableRow>
    {
        public ResourceTableAdapter(SqlConnection connection) :
            base(connection, "Resources")
        {

        }
    }
}
