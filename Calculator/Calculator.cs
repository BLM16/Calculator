using System;

namespace Calculator
{
    public static class Calculator
    {
        public static string Calculate(string eq)
        {
            eq = Standardizer.Standardize(eq);
            return eq;
        }
    }
}
