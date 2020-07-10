using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTime = DateTime.Now.AddMinutes(1);

            Console.ReadKey();
            DateTime date = DateTime.Now;
            if (date > dateTime)
            {
                Console.WriteLine("YES");
            }
            else
            {
                Console.WriteLine("NO");
            }

            Console.ReadKey();
        }
    }
}
