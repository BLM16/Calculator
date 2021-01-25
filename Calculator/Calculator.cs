using System;
using System.Text.RegularExpressions;

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

            // Find any errors in the equation
            // Invalid chars, operators, etc
            FindErrors(eq);

            // Solve the standardized equation
            var res = Solve(eq);

            return res;
        }

        /// <summary>
        /// Finds any syntax errors that will break our Calculator otherwise.
        /// </summary>
        /// <param name="eq">Equation to check for errors in.</param>
        public static void FindErrors(string eq)
        {
            if (eq.Length < 1)
                throw new MathSyntaxError("Equation too short to be valid");

            for (int i = 0; i < eq.Length; i++)
            {
                // Finds illegal chars
                if (!"()^@#/*+-0123456789.".Contains(eq[i].ToString()))
                    throw new MathSyntaxError($"Invalid char - @Ch:{i + 1}");

                // Finds square root values that aren't wrapped in brackets
                // Valid: @(x) - Invalid: @x - Invalid: @
                if (eq[i] == '@' && (i + 1 >= eq.Length || (eq[i + 1] != '(')))
                    throw new MathSyntaxError($"Square root contents must be wrapped in brackets - @Ch:{i + 1}");

                // Finds decimal points that aren't followed by a number
                // Valid: x.xx - Invalid: x. - Invalid: x./
                if (eq[i] == '.' && (i + 1 > eq.Length || (!"0123456789".Contains(eq[i + 1].ToString()))))
                    throw new MathSyntaxError($"Decimals must be followed by valid digits - @Ch:{i + 1}");

                // Finds consecutive operators
                // Valid: x + x - Invalid: x ++ x
                if ("^/*+-".Contains(eq[i].ToString()) && (i + 1 > eq.Length || "^/*+-".Contains(eq[i + 1].ToString())))
                    throw new MathSyntaxError($"Consecutive operators - @Ch:{i + 1}");
            }
        }

        /// <summary>
        /// Solves the math in a string following BEDMAS rules.
        /// </summary>
        /// <param name="eq">The equation to solve.</param>
        /// <returns>The solved equation.</returns>
        public static string Solve(string eq)
        {
            var bracketPattern = new Regex(@"(?<!@)\((?<eq>\d+(\.\d+)?([\^@/\*\+\-])?\d+([\^@/\*\+\-0-9.]+)?)\)");
            var numberBracketPattern = new Regex(@"(?<!@)\((?<num>\d+(\.\d+)?)\)");

            // Convert the standardized pi symbol to Math.PI
            eq = eq.Replace("#", Math.PI.ToString());

            foreach (Match match in numberBracketPattern.Matches(eq))
                eq = eq.Replace(match.Value, match.Groups["num"].Value);

            // Cycle through all the brackets following BEDMAS order and replace them with their solved version
            while (true)
            {
                var bracketEquation = bracketPattern.Match(eq);

                if (bracketEquation.Length == 0)
                    break;

                eq = eq.Replace(bracketEquation.Value, Solve(bracketEquation.Value.Substring(1, bracketEquation.Length - 2)));
            }
            
            // Call the local functions to evaluate the equation following BEDMAS order
            eq = Sqrt(eq);
            eq = Exponants(eq);
            eq = DivMult(eq);
            eq = AddSub(eq);

            while (true)
            {
                if (bracketPattern.Matches(eq).Count > 0)
                    eq = Solve(eq);
                else
                    break;
            }

            return eq;
        }

        /// <summary>
        /// Solves the square roots in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        private static string Sqrt(string eq)
        {
            //var re = new Regex(@"@\((?<num>\d+(\.\d+)?)\)");
            var sqrtRexex = new Regex(@"@\((?<eq>\d+(\.\d+)?([\^@/\*\+\-0-9.]+)?)\)");
            var matches = sqrtRexex.Matches(eq);

            foreach (Match match in matches)
            {
                //eq = eq.Replace(match.Value, Solve(match.Groups["eq"].Value));

                var num = double.Parse(Solve(match.Groups["eq"].Value));
                eq = eq.Replace(match.Value, Math.Sqrt(num).ToString());
            }

            while (true)
            {
                if (sqrtRexex.Matches(eq).Count > 0)
                    eq = Sqrt(eq);
                else
                    break;
            }

            return eq;
        }

        /// <summary>
        /// Solves the exponents in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        private static string Exponants(string eq)
        {
            var exponantRegex = new Regex(@"(?<num1>\d+(\.\d+)?)\^(?<num2>\d+(\.\d+)?)");
            var matches = exponantRegex.Matches(eq);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                var nums = new double[] { double.Parse(groups["num1"].Value), double.Parse(groups["num2"].Value) };
                eq = eq.Replace(match.ToString(), Math.Pow(nums[0], nums[1]).ToString());
            }

            while (true)
            {
                if (exponantRegex.Matches(eq).Count > 0)
                    eq = Exponants(eq);
                else
                    break;
            }

            return eq;
        }

        /// <summary>
        /// Solves the division and multiplication in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        private static string DivMult(string eq)
        {
            var divMultRegex = new Regex(@"(?<num1>\d+(\.\d+)?)(?<op>[\*\/])(?<num2>\d+(\.\d+)?)");
            var matches = divMultRegex.Matches(eq);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                var op = groups["op"].Value;
                var nums = new double[] { double.Parse(groups["num1"].Value), double.Parse(groups["num2"].Value) };

                // Handle divide by 0 errors
                if (nums[1] == 0 && op == "/")
                    throw new DivideByZeroException($"can\'t divide {nums[0]} by 0");

                eq = eq.Replace(match.ToString(), op == "*" ?
                    (nums[0] * nums[1]).ToString() :
                    (nums[0] / nums[1]).ToString());
            }

            while (true)
            {
                if (divMultRegex.Matches(eq).Count > 0)
                    eq = DivMult(eq);
                else
                    break;
            }

            return eq;
        }

        /// <summary>
        /// Solves the addition and subtraction in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        private static string AddSub(string eq)
        {
            var addSubRegex = new Regex(@"(?<num1>\d+(\.\d+)?)(?<op>[\+\-])(?<num2>\d+(\.\d+)?)");
            var matches = addSubRegex.Matches(eq);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                var op = groups["op"].Value;
                var nums = new double[] { double.Parse(groups["num1"].Value), double.Parse(groups["num2"].Value) };

                eq = eq.Replace(match.ToString(), op == "+" ?
                    (nums[0] + nums[1]).ToString() :
                    (nums[0] - nums[1]).ToString());
            }

            while (true)
            {
                if (addSubRegex.Matches(eq).Count > 0)
                    eq = AddSub(eq);
                else
                    break;
            }

            return eq;
        }
    }
}
