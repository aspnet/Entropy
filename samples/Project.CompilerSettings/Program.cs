using System;

namespace Project.CompilerSettings
{
    public class Program
    {
        public static void Main()
        {
#if SOMETHING
            Console.WriteLine("Something");
#else
            Console.WriteLine("Nothing");
#endif
            Console.ReadLine();
        }

        public unsafe void UnsafeStuff()
        {
            string s = "Hello";

            fixed (char* ptr = s)
            {

            }
        }
    }
}
