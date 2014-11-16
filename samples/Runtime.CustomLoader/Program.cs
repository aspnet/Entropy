using System;
using System.IO;
using System.Reflection;
using Microsoft.Framework.Runtime;

namespace Runtime.CustomLoader
{
    public class Program
    {
        private readonly IAssemblyLoadContextAccessor _loadContextAccessor;
        private readonly IAssemblyLoaderContainer _loaderContainer;
        private readonly IApplicationShutdown _shutdown;

        public Program(IAssemblyLoaderContainer container,
                       IAssemblyLoadContextAccessor accessor,
                       IApplicationShutdown shutdown)
        {
            _loaderContainer = container;
            _loadContextAccessor = accessor;
            _shutdown = shutdown;
        }

        public void Main(string[] args)
        {
            // Use the load context of the current assembly (the default context)
            var loadContext = _loadContextAccessor.GetLoadContext(typeof(Program).GetTypeInfo().Assembly);

            // Add the loader to the container so that any call to Assembly.Load will
            // call the load context back (if it's not already loaded)
            using (_loaderContainer.AddLoader(new DirectoryLoader(@"", loadContext)))
            {
                // You should be able to use Assembly.Load()
                var assembly1 = Assembly.Load(new AssemblyName("SomethingElse"));

                // Or call load on the context directly
                var assembly2 = loadContext.Load("SomethingElse");

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

        public Assembly Load(string name)
        {
            return _context.LoadFile(Path.Combine(_path, name + ".dll"));
        }
    }
}
