using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Calculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [DataTestMethod]
        [DataRow("156(piroot(7))^2", "10777.608005989581")]
        [DataRow("156(pi3)root(7+19)^2", "38226.8994088806")]
        public void Calculate(string equation, string expected)
        {
            var actual = Calculator.Calculate(equation);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("@((4+4)^2)", "8")]
        [DataRow("@(@(32+9^2))", "3.260390438695134")]
        [DataRow("@(@(@(81)))", "1.7320508075688772")]
        [DataRow("@(16)*2", "8")]
        public void SqrtTest(string equation, string expected)
        {
            var actual = Calculator.Solve(equation);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("2^2^2", "16")]
        [DataRow("3*3*3", "27")]
        [DataRow("2+2+7", "11")]
        public void DoublesTest(string equation, string expected)
        {
            var actual = Calculator.Calculate(equation);
            Assert.AreEqual(expected, actual);
        }

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
        {
            var equation = "321/0";
            Calculator.Solve(equation);
        }

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
