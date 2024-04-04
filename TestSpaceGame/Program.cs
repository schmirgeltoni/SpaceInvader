using System;
using System.Text;

namespace TestSpaceGame
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            for (int i = 0x2550; i <= 0x256C;  i++)
            {
            Console.WriteLine(Convert.ToChar(i));
            }
        }
        
    }
}