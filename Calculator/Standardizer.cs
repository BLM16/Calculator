﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Class to standardize math in a string.
    /// </summary>
    public static class Standardizer
    {
        /// <summary>
        /// Handles calling the required methods in order to properly standardize the math in a string.
        /// </summary>
        /// <param name="eq">The equation to standardize.</param>
        /// <returns>The standardized string.</returns>
        public static string Standardize(string eq)
        {
            // Remove spaces from the equation because they are irrelevant
            eq = eq.Replace(" ", "");

            // Call local functions to standardize the equation
            eq = FixBrackets(eq);
            eq = ReplaceSpecChars(eq);
            eq = AddMultSigns(eq);

            return eq;
        }

        /// <summary>
        /// <para>Fixes brackets in an equation by adding ones to the end if possible.</para>
        /// <para>Throws an error for unmatching brackets otherwise.</para>
        /// </summary>
        /// <param name="eq">The equation to be fixed.</param>
        /// <returns>The fixed equation.</returns>
        public static string FixBrackets(string eq)
        {
            // Counters for the number of each bracket
            int lBrack = 0, rBrack = 0;

            // Strip all '(' at the end of the equation if any
            var endLBrackPattern = new Regex(@"\(+$");
            eq = endLBrackPattern.Replace(eq, "");

            // Count the number of each bracket
            foreach (var c in eq)
            {
                if (c == '(') lBrack++;
                if (c == ')') rBrack++;
            }

            // Fix brackets where possible or throw a MathSyntaxError
            if (rBrack > lBrack)
                throw new MathSyntaxError("Too many closing brackets");
            else if (lBrack > rBrack)
                for (int i = rBrack; i < lBrack; i++)
                    eq += ")";

            return eq;
        }

        /// <summary>
        /// Replaces certain operators that can be written in different ways with a standard.
        /// </summary>
        /// <param name="eq">The equation to standardize operators for.</param>
        /// <returns>The operator standardized equation.</returns>
        public static string ReplaceSpecChars(string eq)
        {
            // Replace square root "operators" with a standard: @
            eq = eq.ToLower().Replace("root", "@");
            eq = eq.ToLower().Replace("sqrt", "@");
            eq = eq.Replace("√", "@");


            // Replace pi "operators" with a standard: #
            eq = eq.ToLower().Replace("pi", "#");
            eq = eq.Replace("π", "#");

            // Replace multiplication and division signs with the standards: * and /
            eq = eq.Replace("×", "*");
            eq = eq.Replace("÷", "/");

            return eq;
        }

        /// <summary>
        /// Adds multiplication signs where mathematically a multiplication would be performed. <code>34(2)</code> becomes <code>34*(2)</code>
        /// </summary>
        /// <param name="eq">The equation to add multiplication operators to.</param>
        /// <returns>The equation with the added multiplication signs.</returns>
        public static string AddMultSigns(string eq)
        {
            List<char> standard = new List<char>(eq.ToCharArray());
            string nums = "0123456789";

            // For each char in the equation check if a multiplication sign needs to be inserted
            for (int i = 0; i < standard.Count; i++)
            {
                if (standard[i] == '(' && i - 1 >= 0 && (nums.Contains(standard[i - 1]) || standard[i - 1] == ')'))
                    standard.Insert(i, '*');

                if (nums.Contains(standard[i]) && i - 1 >= 0 && standard[i - 1] == ')')
                    standard.Insert(i, '*');

                if (standard[i] == '@' && i - 1 >= 0 && (nums.Contains(standard[i - 1]) || standard[i - 1] == ')' || standard[i - 1] == '#'))
                    standard.Insert(i, '*');

                if (standard[i] == '#' && i - 1 >= 0 && (nums.Contains(standard[i - 1]) || standard[i - 1] == ')'))
                    standard.Insert(i, '*');

                if ((nums.Contains(standard[i]) || standard[i] == '(') && i - 1 >= 0 && standard[i - 1] == '#')
                    standard.Insert(i, '*');
            }

            return string.Join("", standard);
        }
    }
}
