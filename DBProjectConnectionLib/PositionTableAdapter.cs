using System.Data.SqlClient;

namespace DBProjectConnectionLib
{
    public class PositionTableAdapter:TableAdapter<PositionTableRow>
    {
        public PositionTableAdapter(SqlConnection connection)
            : base(connection, "Position")
        {

        }
    }
}
