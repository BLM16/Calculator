using System;

namespace Calculator
{
    /// <summary>
    /// Class calculates math in a string. Calls required methods in order.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Handles calling the required methods in order to properly calculate the math in a string.
        /// </summary>
        /// <param name="eq">The equation to calculate.</param>
        /// <returns>The calculated answer as a string.</returns>
        public static string Calculate(string eq)
        {
            // Standardize the string to optimize the Evaluator
            eq = Standardizer.Standardize(eq);

            return eq;
        }
    }
}
