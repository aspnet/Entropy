using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace Content.Upload.Files
{
    public class Startup
    {
        // Note that IIS & IIS Express limit request bodies to 4mb by default. To raise the limit for IIS express change the following:
        // (Current User)\Documents\IISExpress\config\applicationhost.config
        // <configuration>
        //   <system.webServer>
        //     <security>
        //       <requestFiltering>
        //         <requestLimits maxAllowedContentLength = "1000000000" /
        // Restart the website.
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>");

                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();
                    await context.Response.WriteAsync("Form received: " + form.Count() + " entries.<br>");

                    foreach (var part in form)
                    {
                        await context.Response.WriteAsync("- Key: " + part.Key + "; Value(s): " + string.Join(", ", part.Value) + "<br>");
                    }

                    await context.Response.WriteAsync("<br>");

                    await context.Response.WriteAsync("Files received: " + form.Files.Count + " entries.<br>");

                    foreach (var file in form.Files)
                    {
                        await context.Response.WriteAsync("- Content-Disposition: " + file.ContentDisposition + "<br>");
                        await context.Response.WriteAsync(" - Length: " + file.Length + "<br>");
                        var body = file.OpenReadStream();
                        // Consume the file body
                    }
                }

                await context.Response.WriteAsync("<br>Form:");

                await context.Response.WriteAsync(@"
<FORM action = ""/"" method=""post"" enctype=""multipart/form-data"" >
<P>
<LABEL for=""firstname"">Description:</LABEL>
<INPUT type=""text"" name=""description"" value=""Foo"" /><BR>
<LABEL for=""myfile1"">File 1:</LABEL>
<INPUT type=""file"" name=""myfile1"" /><BR>
<LABEL for=""myfile2"">File 2:</LABEL>
<INPUT type=""file"" name=""myfile2"" /><BR>
<INPUT type=""submit"" value=""Send"" />
</P>
</FORM>");

                await context.Response.WriteAsync("</body></html>");
            });
        }

        public static void Main(string[] args)
        {
            var application = new WebApplicationBuilder()
                .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
