using System;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Content.Upload.Files
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>");

                if (string.Equals("POST", context.Request.Method, StringComparison.OrdinalIgnoreCase))
                {
                    var form = await context.Request.ReadFormBodyAsync();
                    await context.Response.WriteAsync("Form received: " + form.Count() + " entries.<br>");

                    foreach (var part in form)
                    {
                        await context.Response.WriteAsync("- Key: " + part.Key + "; Value(s): " + string.Join(", ", part.Value) + "<br>");
                    }

                    await context.Response.WriteAsync("<br>");

                    var files = await context.Request.ReadFilesBodyAsync();
                    await context.Response.WriteAsync("Files received: " + files.Count() + " entries.<br>");

                    foreach (var file in files)
                    {
                        await context.Response.WriteAsync("- Content-Disposition: " + file.ContentDisposition + "<br>");
                        await context.Response.WriteAsync(" - Length: " + file.Body.Length + "<br>");
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
    }
}
