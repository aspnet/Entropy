using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.CompilerSettings
{
    public class Program
    {
        public void Main()
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
