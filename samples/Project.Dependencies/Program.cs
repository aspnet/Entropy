using System;
using System.Threading.Tasks;
#if DNX451
using System.Web;
#endif
using Newtonsoft.Json;
using Project.ProjectReference;
using Project.SharedFiles;

namespace Project.Dependencies
{
    public class Program
    {
        public void Main()
        {
            // Dependency shared
            var data = JsonConvert.SerializeObject(new { message = "Hello World".ToLower2() });

#if DNX451
            // Imported on net45 only
            data = HttpUtility.HtmlEncode(data);
#endif

            // Using shared code
            var tcs = new TaskCompletionSource<object>();
            TaskAsyncHelpers.ContinueWith(Task.Delay(1000), tcs);

            Console.WriteLine(data);

            Console.ReadLine();
        }
    }
}
