using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Class calculates math in a string. Calls required methods in order.
    /// </summary>
    public static class Calculator
    {
        // Define regexes used for solving equations
        private static readonly Regex bracketPattern = new Regex(@"(?<!@)\((?<eq>\d+(\.\d+)?([\^@/\*\+\-])?\d+([\^@/\*\+\-0-9.]+)?)\)");
        private static readonly Regex numberBracketPattern = new Regex(@"(?<!@)\((?<num>\d+(\.\d+)?)\)");
        private static readonly Regex sqrtRexex = new Regex(@"@\((?<eq>\d+(\.\d+)?([\^@/\*\+\-0-9.]+)?)\)");
        private static readonly Regex exponantRegex = new Regex(@"(?<num1>\d+(\.\d+)?)\^(?<num2>\d+(\.\d+)?)");
        private static readonly Regex divMultRegex = new Regex(@"(?<num1>\d+(\.\d+)?)(?<op>[\*\/])(?<num2>\d+(\.\d+)?)");
        private static readonly Regex addSubRegex = new Regex(@"(?<num1>\d+(\.\d+)?)(?<op>[\+\-])(?<num2>\d+(\.\d+)?)");

        /// <summary>
        /// Handles calling the required methods in order to properly calculate the math in a string.
        /// </summary>
        /// <param name="eq">The equation to calculate.</param>
        /// <returns>The calculated answer as a string.</returns>
        public static string Calculate(string eq)
        {
            // Standardize the string to something the calculator can evaluate
            eq = Standardizer.Standardize(eq);

            // Find any errors in the equation
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
            // If the equation length is less than 1, there is no equation
            // Throw a MathSyntaxError because we don't know what to do with it
            if (eq.Length < 1)
                throw new MathSyntaxError("Equation too short to be valid");

            // Loop through for every character in the equation to test for positional based errors
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
            // Convert the standardized pi symbol to Math.PI
            eq = eq.Replace("#", Math.PI.ToString());

            while (true)
            {
                // Remove brackets around just a number like "(86.2)"
                foreach (Match match in numberBracketPattern.Matches(eq))
                    eq = eq.Replace(match.Value, match.Groups["num"].Value);

                // Solve all square roots following BEDMAS
                // Since this returns a value and doesn't affect other numbers, we can do this before brackets
                if (sqrtRexex.Matches(eq).Count > 0)
                    eq = Sqrt(eq);

                if (bracketPattern.Matches(eq).Count > 0)
                {
                    // If there are brackets to solve, solve them
                    var bracketEquation = bracketPattern.Match(eq);
                    eq = eq.Replace(bracketEquation.Value, Solve(bracketEquation.Value.Substring(1, bracketEquation.Length - 2)));
                }
                else
                {
                    // Call local BEDMAS functions to solve the equation
                    eq = Exponants(eq);
                    eq = DivMult(eq);
                    eq = AddSub(eq);

                    return eq;
                }
            }
        }

        /// <summary>
        /// Solves the square roots in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        /// <returns>The equation with square roots evaluated.</returns>
        private static string Sqrt(string eq)
        {
            // Keep looping while there are still square roots in the equation
            while (true)
            {
                if (sqrtRexex.Matches(eq).Count > 0)
                {
                    // Get the first remaining square root's "contents" and evaluate them, then solve the square root
                    var match = sqrtRexex.Match(eq);
                    var num = double.Parse(Solve(match.Groups["eq"].Value));
                    eq = eq.Replace(match.Value, Math.Sqrt(num).ToString());
                }
                else
                    return eq;
            }
        }

        /// <summary>
        /// Solves the exponants in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        /// <returns>The equation with exponants evaluated.</returns>
        private static string Exponants(string eq)
        {
            // Keep looping while there are still exponants in the equation
            while (true)
            {
                if (exponantRegex.Matches(eq).Count > 0)
                {
                    // Get the first remaining exponant
                    var match = exponantRegex.Match(eq);

                    // Parse the numbers as doubles and evaluate the exponant
                    var nums = new double[] { double.Parse(match.Groups["num1"].Value), double.Parse(match.Groups["num2"].Value) };
                    eq = eq.Replace(match.Value, Math.Pow(nums[0], nums[1]).ToString());
                }
                else
                    return eq;
            }
        }

        /// <summary>
        /// Solves the division and multiplication in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        /// <returns>The equation with divisions and multiplications evaluated.</returns>
        private static string DivMult(string eq)
        {
            // Keep looping while there are still divisions or multiplications in the equation
            while (true)
            {
                if (divMultRegex.Matches(eq).Count > 0)
                {
                    // Get the first remaining division or multiplication
                    var match = divMultRegex.Match(eq);

                    // Get the operator: "*" or "/"
                    // Parse the numbers as doubles
                    var op = match.Groups["op"].Value;
                    var nums = new double[] { double.Parse(match.Groups["num1"].Value), double.Parse(match.Groups["num2"].Value) };

                    // Handle divide by 0 errors
                    if (nums[1] == 0 && op == "/")
                        throw new DivideByZeroException($"Can\'t divide {nums[0]} by 0");

                    // Evaluate the multiplication or division
                    eq = eq.Replace(match.Value, op == "*" ?
                        (nums[0] * nums[1]).ToString() :
                        (nums[0] / nums[1]).ToString());
                }
                else
                    return eq;
            }
        }

        /// <summary>
        /// Solves the addition and subtraction in the given equation.
        /// </summary>
        /// <param name="eq">Equation to solve.</param>
        /// <returns>The equation with additions and subtractions evaluated.</returns>
        private static string AddSub(string eq)
        {
            // Keep looping while there are still additions or subtractions in the equation
            while (true)
            {
                if (addSubRegex.Matches(eq).Count > 0)
                {
                    // Get the first remaining addition or subtraction
                    var match = addSubRegex.Match(eq);

                    // Get the operator: "+" or "-"
                    // Parse the numbers as doubles
                    var op = match.Groups["op"].Value;
                    var nums = new double[] { double.Parse(match.Groups["num1"].Value), double.Parse(match.Groups["num2"].Value) };

                    // Evaluate the addition or subtraction
                    eq = eq.Replace(match.Value, op == "+" ?
                        (nums[0] + nums[1]).ToString() :
                        (nums[0] - nums[1]).ToString());
                }
                else
                    return eq;
            }
        }
    }
}
