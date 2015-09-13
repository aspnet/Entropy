using System;
using System.IO;
using System.Reflection;
using Microsoft.Dnx.Runtime;

namespace Runtime.CustomLoader
{
    public class Program
    {
        private readonly IAssemblyLoadContextAccessor _loadContextAccessor;
        private readonly IAssemblyLoaderContainer _loaderContainer;

        public Program(IAssemblyLoaderContainer container,
                       IAssemblyLoadContextAccessor accessor)
        {
            _loaderContainer = container;
            _loadContextAccessor = accessor;
        }

        public void Main(string[] args)
        {
            // Use the default load context
            var loadContext = _loadContextAccessor.Default;

            // Add the loader to the container so that any call to Assembly.Load will
            // call the load context back (if it's not already loaded)
            using (_loaderContainer.AddLoader(new DirectoryLoader(@"", loadContext)))
            {
                // You should be able to use Assembly.Load()
                var assembly1 = Assembly.Load(new AssemblyName("SomethingElse"));

                // Or call load on the context directly
                var assembly2 = loadContext.Load("SomethingElse");

                foreach (var definedType in assembly1.DefinedTypes)
                {
                    Console.WriteLine("Found type {0}", definedType.FullName);
                }

                Console.ReadLine();
            }
        }
    }

    public class DirectoryLoader : IAssemblyLoader
    {
        private readonly IAssemblyLoadContext _context;
        private readonly string _path;

        public DirectoryLoader(string path, IAssemblyLoadContext context)
        {
            _path = path;
            _context = context;
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            return _context.LoadFile(Path.Combine(_path, assemblyName.Name + ".dll"));
        }
    }
}
