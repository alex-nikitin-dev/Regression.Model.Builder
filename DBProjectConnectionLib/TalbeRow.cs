using System.Collections.Generic;

namespace DBProjectConnectionLib
{
    public abstract class TableRow
    {
        public  string GetValuesWithoutId()
        {
            return GetValues(1);
        }

        public string GetAllValues()
        {
            return GetValues(0);
        }
        public string GetValues(int initialIndex)
        {
            var result = "";
            var type = GetType();
            var properties = GetPropertiesInOrder();
            for (int i = initialIndex; i < properties.Count; i++)
            {
                result += $"'{type.GetField(properties[i]).GetValue(this)}',";
            }

            return result.TrimEnd(',');
        }


        public  void SetValues(List<object> values)
        {
            var type = GetType();
            var properties = GetPropertiesInOrder();
            for (int i = 0; i < properties.Count; i++)
            {
                type.GetField(properties[i]).SetValue(this, values[i]);
            }
        }
        public abstract List<string> GetPropertiesInOrder();
    }
}
