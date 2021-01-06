using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Calculator.Tests
{
    [TestClass()]
    public class StandardizerTests
    {
        /// <summary>
        /// <para>Checks if spaces are removed.</para>
        /// <para>Checks combinations of all other types of equations.</para>
        /// </summary>
        [TestMethod()]
        public void StandardizeTest()
        {
            // Define equations to test and their expected result
            // { equation, expected }
            string[,] eqs = {
                { "34 + 97 * 3 - 7", "34+97*3-7" },
                { "π (root(3(7) / 4) + 2)(97(22-3/4) * 6^2)root((12 + 2) 3) - 3 (12 + 2) √(pi)(", $"{Math.PI}*(@(3*(7)/4)+2)*(97*(22-3/4)*6^2)*@((12+2)*3)-3*(12+2)*@({Math.PI})" },
                { "(24 root(7) + (14^2 - 3) 11", "(24*@(7)+(14^2-3)*11)" }
            };

            // For each equation in eqs verify that the actual result == expected result
            for (int i = 0; i < eqs.GetLength(0); i++)
            {
                var equation = eqs[i, 0];
                var expected = eqs[i, 1];

                var actual = Standardizer.Standardize(equation);

                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// <para>Checks if removing trailing '(' works.</para>
        /// <para>Checks if ')' are added to even the number of '(' brackets.</para>
        /// <para>Checks if too many closing brackets throws an ArithmeticException.</para>
        /// </summary>
        [TestMethod()]
        public void FixBracketsTest()
        {
            // Define equations to test and their expected result
            // { equation, expected }
            string[,] eqs = {
                { "32(97-8+84)(", "32(97-8+84)" },
                { "365(37+94(32", "365(37+94(32))" }
            };

            // For each equation in eqs verify that the actual result == expected result
            for (int i = 0; i < eqs.GetLength(0); i++)
            {
                var equation = eqs[i, 0];
                var expected = eqs[i, 1];

                var actual = Standardizer.FixBrackets(equation);

                Assert.AreEqual(expected, actual);
            }

            // Verify that having too many ')' throws an ArithmeticException
            Assert.ThrowsException<BracketsException>(() => Standardizer.FixBrackets("(32*7)+4(3)+17)"));
        }

        /// <summary>
        /// <para>Checks if root and √ are replaced with @.</para>
        /// <para>Checks if pi and π are replaced with 3.1415...</para>
        /// </summary>
        [TestMethod()]
        public void ReplaceSpecCharsTest()
        {
            // Define equations to test and their expected result
            // { equation, expected }
            string[,] eqs = {
                { "(35+7)root(9)", "(35+7)@(9)" },
                { "(35+7)√(9)", "(35+7)@(9)" },
                { "14(32-7)/pi", $"14(32-7)/{Math.PI}" },
                { "14(32-7)/π", $"14(32-7)/{Math.PI}" }
            };

            // For each equation in eqs verify that the actual result == expected result
            for(int i = 0; i < eqs.GetLength(0); i++)
            {
                var equation = eqs[i, 0];
                var expected = eqs[i, 1];

                var actual = Standardizer.ReplaceSpecChars(equation);

                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// <para>Checks if x(y) == x*(y).</para>
        /// <para>Checks if (x)(y) == (x)*(y).</para>
        /// <para>Checks if x@(y) == x*@(y).</para>
        /// <para>Checks if (x)@(y) == (x)*@(y).</para>
        /// <para>Checks if (x)y == (x)*y.</para>
        /// </summary>
        [TestMethod()]
        public void AddMultSignsTest()
        {
            // Define equations to test and their expected result
            // { equation, expected }
            string[,] eqs = {
                { "36(2+3(7))", "36*(2+3*(7))" },
                { "(14-8)(6+7)+9", "(14-8)*(6+7)+9" },
                { "(23+34)+9@(14)", "(23+34)+9*@(14)" },
                { "11-(2+9)@(7)", "11-(2+9)*@(7)" },
                { "17+(32-14)4", "17+(32-14)*4" }
            };

            // For each equation in eqs verify that the actual result == expected result
            for (int i = 0; i < eqs.GetLength(0); i++)
            {
                var equation = eqs[i, 0];
                var expected = eqs[i, 1];

                var actual = Standardizer.AddMultSigns(equation);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
