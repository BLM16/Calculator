using System;

namespace App
{
    class Program
    {
        /// <summary>
        /// Implements the Calculator library as an example of usage with I/O.
        /// </summary>
        static void Main()
        {
            // Output instructions
            Console.WriteLine("Accepted values: ( ) ^ root() pi / * + -");
            Console.Write("Enter your equation: ");

            // Take the equation as input
            var eq = Console.ReadLine();

            Console.WriteLine();

            // Evaluate the math equtation
            var res = Calculator.Calculator.Calculate(eq);

            // Output the results
            Console.WriteLine(res);
        }
    }
}
