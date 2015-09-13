using System;
using Microsoft.Dnx.Runtime;

namespace Runtime.ApplicationEnvironment
{
    public class Program
    {
        private readonly IApplicationEnvironment _environment;

        public Program(IApplicationEnvironment environment)
        {
            _environment = environment;
        }

        public void Main()
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("ApplicationName: {0}", _environment.ApplicationName);
            Console.WriteLine("ApplicationBasePath: {0}", _environment.ApplicationBasePath);
            Console.WriteLine("TargetFramework: {0}", _environment.RuntimeFramework);
            Console.WriteLine("Version: {0}", _environment.ApplicationVersion);
            Console.WriteLine("======================================================");
            Console.ReadLine();
        }
    }
}
