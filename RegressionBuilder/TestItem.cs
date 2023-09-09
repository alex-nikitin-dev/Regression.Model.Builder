namespace RegressionBuilder
{
    class TestItem
    {
        public int A { get; internal set; }
        public int B { get; internal set; }

        public TestItem(int a, int b)
        {
            A = a;
            B = b;
        }
    }
}
