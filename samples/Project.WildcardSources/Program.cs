using System;
using System.IO;
using System.Reflection;

namespace Project.WildcardSources
{
    public class Program
    {
        public static void Main()
        {
            var me = Assembly.Load(new AssemblyName("Project.WildcardSources"));

            using (var stream = me.GetManifestResourceStream("Project.WildcardSources.embed.SomeText.txt"))
            {
                var sr = new StreamReader(stream);
                Console.WriteLine(sr.ReadToEnd());
            }

            Console.ReadLine();
        }
    }
}
