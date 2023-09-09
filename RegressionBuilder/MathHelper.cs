using System;
using System.Numerics;

namespace RegressionBuilder
{
    public class MathHelper
    {
        

        public static double Exp(double value, uint n)
        {
            var result = 1.0;
            for (int i = 0; i < n; i++)
            {
                result *= value;
            }

            return result;
        }
       

        public static int GetDecimalPlaces(decimal value)
        {
            var fraction = value - (decimal)new BigInteger(value);
            int n = 0;
            while (fraction > 0)
            {
                value *= 10;
                fraction = value - (decimal)new BigInteger(value);
                n++;
            }
            return n;
        }
    }
}
