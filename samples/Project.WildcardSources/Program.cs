using System;
using System.IO;
using System.Reflection;

namespace Project.WildcardSources
{
    public class Program
    {
        public void Main()
        {
            var me = Assembly.Load("Project.WildcardSources");

            using (var stream = me.GetManifestResourceStream("SomeText.txt"))
            {
                var sr = new StreamReader(stream);
                Console.WriteLine(sr.ReadToEnd());
            }

            Console.ReadLine();
        }
    }
}
