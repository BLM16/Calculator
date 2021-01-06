using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    class Standardizer
    {
        public static string Standardize(string eq)
        {
            eq = eq.Replace(" ", "");

            eq = FixBrackets(eq);
            eq = ReplaceSpecChars(eq);
            eq = AddMultSigns(eq);

            return eq;
        }

        /// <summary>
        /// Fixes brackets in an equation by adding ones to the end if possible.
        /// Else it throws an error for unmatching brackets.
        /// </summary>
        /// <param name="eq">The equation to be fixed.</param>
        /// <returns>The fixed equation.</returns>
        private static string FixBrackets(string eq)
        {
            int lBrack = 0, rBrack = 0;
            char[] eqList = eq.ToCharArray();

            // Count the number of brackets
            foreach (char c in eqList)
            {
                if (c == '(') lBrack++;
                if (c == ')') rBrack++;
            }

            // Fix brackets where possible or throw illegal syntax error
            if (rBrack > lBrack)
                throw new ArithmeticException("Brackets don't match");
            else if (lBrack > rBrack)
                for (int i = rBrack; i < lBrack; i++)
                {
                    eq += ")";
                }

            return eq;
        }

        /// <summary>
        /// Replaces certain operators that can be written in different ways with a standard.
        /// </summary>
        /// <param name="eq">The equation to standardize operators for.</param>
        /// <returns>The operator standardized equation.</returns>
        private static string ReplaceSpecChars(string eq)
        {
            // Replace square root "operators" with a standard: @
            eq = eq.Replace("root", "@");
            eq = eq.Replace("√", "@");

            // Replace pi "operators" with pi's value
            eq = eq.Replace("pi", Math.PI.ToString());
            eq = eq.Replace("π", Math.PI.ToString());

            return eq;
        }

        /// <summary>
        /// Adds multiplication signs where mathematically a multiplication would be performed. <code>34(2)</code> becomes <code>34*(2)</code>
        /// </summary>
        /// <param name="eq">The equation to add multiplication operators to.</param>
        /// <returns>The equation with the added multiplication signs.</returns>
        private static string AddMultSigns(string eq)
        {
            List<char> standard = new List<char>(eq.ToCharArray());
            string nums = "0123456789";

            // For each char in the equation check if a multiplication sign needs to be inserted
            for (int i = 0; i < standard.Count; i++)
            {
                if (standard[i] == '(' && i - 1 >= 0 && nums.Contains(standard[i - 1]))
                    standard.Insert(i, '*');

                if (standard[i] == '@' && i - 1 >= 0 && nums.Contains(standard[i - 1]))
                    standard.Insert(i, '*');

                if (nums.Contains(standard[i]) && i - 1 >= 0 && standard[i - 1] == ')')
                    standard.Insert(i, '*');

                if (standard[i] == '@' && i - 1 >= 0 && standard[i - 1] == ')')
                    standard.Insert(i, '*');
            }

            eq = string.Join("", standard);

            return eq;
        }
    }
}
