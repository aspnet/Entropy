using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.AsyncEntryPoint
{
    public class Program
    {
        public static async Task<int> Main()
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                Console.WriteLine(i);
            }

#if ASPNET50
            Console.ReadKey();
#endif

            return 0;
        }
    }
}
