using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Content.Upload.Form
{
    public class Startup
    {
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

                    // Once the form has been read we can access it directly.
                    foreach (var part in context.Request.Form)
                    {
                        await context.Response.WriteAsync("- Key: " + part.Key + "; Value(s): " + string.Join(", ", part.Value) + "<br>");
                    }
                }

                await context.Response.WriteAsync("<br>Form:");
                await context.Response.WriteAsync(@"
<FORM action = ""/"" method = ""post"">
<P>
<LABEL for=""firstname"">First name:</LABEL>
<INPUT type=""text"" name=""firstname"" value=""Foo"" /><BR>
<LABEL for=""lastname"">Last name:</LABEL>
<INPUT type=""text"" name=""lastname"" value=""Bar"" /><BR>
<LABEL for=""email"">email:</LABEL>
<INPUT type=""text"" name=""email"" value=""Foo@Bar"" /><BR>
<INPUT type=""radio"" name=""sex"" value=""Male"" />Male<BR>
<INPUT type=""radio"" name=""sex"" value=""Female"" />Female<BR>
<INPUT type=""submit"" value=""Send"" /><INPUT type=""reset"" />
</P>
</FORM>");
                await context.Response.WriteAsync("</body></html>");
            });
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultHostingConfiguration(args)
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

