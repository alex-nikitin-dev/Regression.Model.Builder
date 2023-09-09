using System.Data.SqlClient;

namespace DBProjectConnectionLib
{
    public class ProjectsTableAdapter : TableAdapter<ProjectsTableRow>
    {
        public ProjectsTableAdapter(SqlConnection connection):
            base(connection,"Projects")
        {
           
        }
    }
}
