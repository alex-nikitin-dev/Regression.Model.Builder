using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBProjectConnectionLib
{
    public abstract class Table<TRow>:IEnumerable<TRow> where TRow:TableRow
    {
        protected List<TRow> _rows;

        public void Set(List<TRow> rows)
        {
            _rows = new List<TRow>(rows);
        }

        public IEnumerator<TRow> GetEnumerator()
        {
            return new TableEnum<TRow>(_rows);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
