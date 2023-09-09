namespace RegressionBuilder
{
    class StudentsTDistributionTable:TabularItemsTable
    {
        private StudentsTDistributionTable(string path) : base(path)
        {
        }

        public static StudentsTDistributionTable GetInstance()
        {
            return new StudentsTDistributionTable(RegressionBuilder.Properties.Settings.Default.StudentsTDistributionPath);
        }
    }
}
