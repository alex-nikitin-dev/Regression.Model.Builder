using System.Collections.Generic;
using System.Data.SqlClient;
namespace DBProjectConnectionLib
{
    public abstract class TableAdapter<TRowType> where TRowType:TableRow, new()
    {
        public  SqlConnection Connection { get; protected set; }
        public  string TableName { get; }
        protected  string InsertQuery { get; set; }
        protected string SelectAllQuery { get; set; }
        public  List<string> AllColumnsNames { get; protected set; }

       
        protected string GetAllColumnsNamesBySeparator(char separator,bool isIdNeeded)
        {
            var result = "";
            for (int i = (isIdNeeded ? 0 : 1); i < AllColumnsNames.Count; i++)
            {
                result += $"{AllColumnsNames[i]}{separator}";
            }

            return result.TrimEnd(separator);
        }

        protected string GenerateInsertQuery(string values)
        {
            var columns = GetAllColumnsNamesBySeparator(',',false);
            return  $"INSERT INTO {TableName} ({columns}) " +
                    $"VALUES ({values})";
        }

        protected string GenerateSelectAllQuery()
        {
            return $"Select {GetAllColumnsNamesBySeparator(',', true)}  FROM {TableName}";
        }

        public void DropAllRows()
        {
            var query = $"Delete From {TableName}";
            new SqlCommand(query, Connection).ExecuteNonQuery();
        }

        public  void Insert(TRowType row)
        {
            var query = GenerateInsertQuery(row.GetValuesWithoutId());
            new SqlCommand(query,Connection).ExecuteNonQuery();
        }

        public  List<TRowType> SelectAll()
        {
            var list = new List<TRowType>();
            using (var reader = new SqlCommand(SelectAllQuery, Connection).ExecuteReader())
            {
                while (reader.Read())
                {
                    var values = new List<object>();

                    for (int i = 0; i < AllColumnsNames.Count; i++)
                    {
                        values.Add(reader[AllColumnsNames[i]]);
                    }

                    var row = new TRowType();
                    row.SetValues(values);
                    list.Add(row);
                }
            }

            return list;
        }

        protected TableAdapter(SqlConnection connection, string tableName)
        {
            TableName = tableName;
            Connection = connection;
            AllColumnsNames = new TRowType().GetPropertiesInOrder();
            SelectAllQuery = GenerateSelectAllQuery();
        }

        public void Fill(Table<TRowType> table)
        {
            table.Set(SelectAll());
        }
    }
}
