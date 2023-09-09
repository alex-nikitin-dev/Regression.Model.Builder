using RegressionBuilder.Properties;

namespace RegressionBuilder
{
    class  ChiCriticalTable:TabularItemsTable
    {
        private ChiCriticalTable(string path) : base(path)
        {
        }
        public static ChiCriticalTable GetChiCriticalTable()
        {
            return new ChiCriticalTable(Settings.Default.ChiCriticalPath);
        }
    }
}
