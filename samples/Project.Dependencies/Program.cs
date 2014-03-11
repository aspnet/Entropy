using System;
using System.Web;
using Newtonsoft.Json;
using Project.ProjectReference;

namespace Project.Dependencies
{
    public class Program
    {
        public void Main()
        {
            var data = JsonConvert.SerializeObject(new { message = "Hello World".ToLower2() });

#if NET45
            data = HttpUtility.HtmlEncode(data);
#endif
            
            Console.WriteLine(data);

            Console.ReadLine();
        }
    }
}
