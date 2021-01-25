using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Calculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        #region Calculate

        /// <summary>
        /// Checks that equations are correctly standardized and evaluated.
        /// </summary>
        [TestMethod]
        public void Calculate_EquationsAreStandardizedCorrectly()
        {
            var equation = "(97 - 27) / (7) + 14";
            var expected = "24";

            var actual = Calculator.Calculate(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that equations containing brackets are correctly evaluated.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("156(piroot(7))^2", "10777.608005989581")]
        [DataRow("156(pi3)root(7+19)^2", "38226.8994088806")]
        [DataRow("7(9+2)(5-2)/3", "77")]
        public void Calculate_NestedEquationsEvalCorrectly(string equation, string expected)
        {
            var actual = Calculator.Calculate(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that equations containing multiple consecutive operations are evaluated correctly.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("@(@(32+9^2))", "3.260390438695134")]
        [DataRow("2^2^2", "16")]
        [DataRow("27/3/3", "3")]
        [DataRow("3*3*3", "27")]
        [DataRow("2+2+7", "11")]
        [DataRow("47-22-3", "22")]
        public void Calculate_MultiplesEvalCorrectly(string equation, string expected)
        {
            var actual = Calculator.Calculate(equation);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region FindErrors

        /// <summary>
        /// Checks that illegal chars throw a MathSyntaxError.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        [DataTestMethod]
        [DataRow("32x+97*(34)")]
        [DataRow("13^(7)+2!")]
        [ExpectedException(typeof(MathSyntaxError))]
        public void FindErrors_ExceptionOnIllegalChars(string equation)
            => Calculator.FindErrors(equation);

        /// <summary>
        /// Checks that root signs without brackets wrapping their contents throws a MathSyntaxError.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        [DataTestMethod]
        [DataRow("13+@27")]
        [DataRow("12*@")]
        [DataRow("@")]
        [ExpectedException(typeof(MathSyntaxError))]
        public void FindErrors_ExceptionOnRootWithNoBrackets(string equation)
            => Calculator.FindErrors(equation);

        /// <summary>
        /// Checks that having a non numeric value following a decimal throws a MathSyntaxError.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MathSyntaxError))]
        public void FindErrors_ExceptionOnNonNumericValueAfterDecimal()
            => Calculator.FindErrors("32.+9");

        [DataTestMethod]
        [DataRow("32++7")]
        [DataRow("19(+4)*/6")]
        [DataRow("7^^6")]
        [ExpectedException(typeof(MathSyntaxError))]
        public void FindErrors_ExceptionOnConsecutiveOperators(string equation)
            => Calculator.FindErrors(equation);

        #endregion

        #region Solve

        /// <summary>
        /// Checks that equations containing brackets are solved correctly.
        /// </summary>
        /// <param name="equation">Equation to solve.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("2^2*(3^2*(4^2))", "576")]
        [DataRow("2^2*(3^2+2^2)", "52")]
        [DataRow("(2^3)+3^2*(2^3*(2^3+2^2))", "872")]
        [DataRow("(132)", "132")]
        public void Solve_BracketEquations(string equation, string expected)
        {
            var actual = Calculator.Solve(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that equations containing exponents are solved correctly.
        /// </summary>
        [TestMethod]
        public void Solve_Exponents()
        {
            var equation = "3^5";
            var expected = "243";

            var actual = Calculator.Solve(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that equations containing square roots are solved correctly.
        /// </summary>
        [TestMethod]
        public void Solve_SquareRoots()
        {
            var equation = "@(64)";
            var expected = "8";

            var actual = Calculator.Solve(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that equations containing division, multiplication, addition, or subtraction are solved correctly.
        /// </summary>
        /// <param name="equation">Equation to solve.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("81/9/3", "3")]
        [DataRow("17*4*2", "136")]
        [DataRow("22+117+11", "150")]
        [DataRow("194-167-12", "15")]
        public void Solve_DivisionMultiplicationAdditionSubtraction(string equation, string expected)
        {
            var actual = Calculator.Solve(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that division by zero throws a DivideByZeroException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void Solve_ExceptionOnDivisionByZero()
            => Calculator.Solve("321/(3-3)");

        /// <summary>
        /// Checks that equations containing the pi '#' are converted and solved correctly.
        /// </summary>
        [TestMethod]
        public void Solve_PiGetsConverted()
        {
            Assert.AreEqual("10.2", Calculator.Solve("3.2+7"));
            Assert.AreEqual("9.42477796076938", Calculator.Solve("3*#"));
        }

        #endregion
    }
}
