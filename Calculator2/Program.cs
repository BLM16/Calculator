using System;

namespace App
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Accepted values: ( ) ^ root() pi / * + -");
            Console.Write("Enter your equation: ");

            string eq = Console.ReadLine();

            Console.WriteLine();

            var res = Calculator.Calculator.Calculate(eq);

            Console.WriteLine(res);
        }
    }
}
