using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class StandardizerTests
    {
        #region Standardize

        /// <summary>
        /// Checks that spaces are removed.
        /// </summary>
        [TestMethod]
        public void Standardize_RemovesSpaces()
        {
            var equation = "34 + 97 * 3 - 7";
            var expected = "34+97*3-7";

            var actual = Standardizer.Standardize(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that combinations of all the standardization methods get standardized correctly together.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("π (root(3(7) / 4) + 2)(97(22-3/4) * 6^2)root((12 + 2) 3) - 3 (12 + 2) √(pi)(", "#*(@(3*(7)/4)+2)*(97*(22-3/4)*6^2)*@((12+2)*3)-3*(12+2)*@(#)")]
        [DataRow("(24 root(7) + (14^2 - pi3) 11pi", "(24*@(7)+(14^2-#*3)*11*#)")]
        public void Standardize_CombinationsStandardizeCorrectly(string equation, string expected)
        {
            var actual = Standardizer.Standardize(equation);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region FixBrackets

        /// <summary>
        /// Checks that missing right brackets are automatically appended.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("365(37+94", "365(37+94)")] // Single missing right bracket
        [DataRow("365(37+94(32", "365(37+94(32))")] // Multiple missing right brackets
        public void FixBrackets_AppendsRightBrackets(string equation, string expected)
        {
            var actual = Standardizer.FixBrackets(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that stripping end '(' works.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("32(97-8+84)(", "32(97-8+84)")] // One trailing left bracket
        [DataRow("32(97-8+84)((", "32(97-8+84)")] // Multiple trailing left brackets
        public void FixBrackets_StripsEndLeftBrackets(string equation, string expected)
        {
            var actual = Standardizer.FixBrackets(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that having too many ')' throws a MathSyntaxError.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MathSyntaxError))]
        public void FixBrackets_ExceptionOnTooManyRightBracks()
        {
            var equation = "(32*7)+4(3)+17)";
            Standardizer.FixBrackets(equation);
        }

        #endregion

        #region ReplaceSpecChars

        /// <summary>
        /// Checks that the case-insensitive word 'root' gets replaced with an '@' sign.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("(35+7)root(9)", "(35+7)@(9)")] // Lowercase word root
        [DataRow("(35+7)ROOT(9)", "(35+7)@(9)")] // Uppercase word root
        [DataRow("(35+7)RoOt(9)", "(35+7)@(9)")] // Mixedcase word root
        public void ReplaceSpecChars_ConvertsWordRootToAtSign(string equation, string expected)
        {
            var actual = Standardizer.ReplaceSpecChars(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that the square root symbol gets replaced with an '@' sign.
        /// </summary>
        [TestMethod]
        public void ReplaceSpecChars_ConvertsRootSignToAtSign()
        {
            var equation = "(35+7)√(9)";
            var expected = "(35+7)@(9)";

            var actual = Standardizer.ReplaceSpecChars(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that the case-insensitive word 'pi' gets replaced with an '#' sign.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("14(32-7)/pi", "14(32-7)/#")] // Lowercase word pi
        [DataRow("14(32-7)/PI", "14(32-7)/#")] // Uppercase word pi
        [DataRow("14(32-7)/Pi", "14(32-7)/#")] // Misedcase word pi
        public void ReplaceSpecChars_ConvertsWordPiToHashtag(string equation, string expected)
        {
            var actual = Standardizer.ReplaceSpecChars(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that the pi symbol gets replaced with an '#' sign.
        /// </summary>
        [TestMethod]
        public void ReplaceSpecChars_ConvertsPiSignToHashtag()
        {
            var equation = "14(32-7)/π";
            var expected = "14(32-7)/#";

            var actual = Standardizer.ReplaceSpecChars(equation);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region AddMultSigns

        /// <summary>
        /// Checks that numbers before and after brackets have multiplication signs inserted.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("36(2+3(7))", "36*(2+3*(7))")]
        [DataRow("17+(32-14)4", "17+(32-14)*4")]
        public void AddMultSigns_NumberBeforeAndAfterBracketsInsertsAsterix(string equation, string expected)
        {
            var actual = Standardizer.AddMultSigns(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that '(x)(y)' get a multiplication sign inserted like '(x)*(y)'.
        /// </summary>
        [TestMethod]
        public void AddMultSigns_ClosingBracketBeforeOpeningBracketInsertsAsterix()
        {
            var equation = "(14-8)(6+7)+9";
            var expected = "(14-8)*(6+7)+9";

            var actual = Standardizer.AddMultSigns(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that '@' after numbers and brackets get a multiplication sign inserted.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("(23+34)+9@(14)", "(23+34)+9*@(14)")]
        [DataRow("11-(2+9)@(7)", "11-(2+9)*@(7)")]
        public void AddMultSigns_RootAfterNumberAndBracketInsertsAsterix(string equation, string expected)
        {
            var actual = Standardizer.AddMultSigns(equation);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that '@' after '#' get a multiplication sign inserted.
        /// </summary>
        [TestMethod]
        public void AddMultSigns_RootAfterPiInsertsAsterix()
        {
            var equation = "155315(#@(14)(37^3))";
            var expected = "155315*(#*@(14)*(37^3))";

            var actual = Standardizer.AddMultSigns(equation);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Checks that '#' after numbers and brackets get a multiplication sign inserted.
        /// </summary>
        /// <param name="equation">Equation to test.</param>
        /// <param name="expected">Expected result.</param>
        [DataTestMethod]
        [DataRow("136#+2#", "136*#+2*#")]
        [DataRow("3+(29^2)#", "3+(29^2)*#")]
        public void AddMultSigns_PiAfterNumberAndBracketInsertsAsterix(string equation, string expected)
        {
            var actual = Standardizer.AddMultSigns(equation);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
