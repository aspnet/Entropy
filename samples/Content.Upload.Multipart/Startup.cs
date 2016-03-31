using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Content.Upload.Multipart
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!IsMultipartContentType(context.Request.ContentType))
                {
                    await next();
                    return;
                }

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Multipart received<br>");

                // Read the request body as multipart sections. This does not buffer the content of each section. If you want to buffer the data
                // then that needs to be added either to the request body before you start or to the individual segments afterward.
                var boundary = GetBoundary(context.Request.ContentType);
                var reader = new MultipartReader(boundary, context.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    await context.Response.WriteAsync("- Header count: " + section.Headers.Count + "<br>");
                    foreach (var headerPair in section.Headers)
                    {
                        await context.Response.WriteAsync("-- " + headerPair.Key + ": " + string.Join(", ", headerPair.Value) + "<br>");
                    }

                    // Consume the section body here.

                    // Nested?
                    if (IsMultipartContentType(section.ContentType))
                    {
                        await context.Response.WriteAsync("-- Nested Multipart<br>");

                        var subBoundary = GetBoundary(section.ContentType);

                        var subReader = new MultipartReader(subBoundary, section.Body);

                        var subSection = await subReader.ReadNextSectionAsync();
                        while (subSection != null)
                        {
                            await context.Response.WriteAsync("--- Header count: " + subSection.Headers.Count + "<br>");
                            foreach (var headerPair in subSection.Headers)
                            {
                                await context.Response.WriteAsync("---- " + headerPair.Key + ": " + string.Join(", ", headerPair.Value) + "<br>");
                            }

                            subSection = await subReader.ReadNextSectionAsync();
                        }
                    }

                    // Drains any remaining section body that has not been consumed and reads the headers for the next section.
                    section = await reader.ReadNextSectionAsync();
                }

                await context.Response.WriteAsync("</body></html>");
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path != new PathString("/SendMultipart"))
                {
                    await next();
                    return;
                }

                // For the purposes of the sample we're going to issue a multipart request to ourselves and then tunnel the response to the user.
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:" + context.Connection.LocalPort);

                var content = new MultipartContent("stuff");
                content.Add(new StringContent("Hello World"));
                content.Add(new FormUrlEncodedContent(new Dictionary<string, string>()));
                content.Add(new MultipartContent("nested") { new StringContent("Nested Hello World") });

                var response = await client.PostAsync("", content);

                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.ContentType = response.Content.Headers.ContentType.ToString();
                await response.Content.CopyToAsync(context.Response.Body);
            });

            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Normal request.<br>");
                await context.Response.WriteAsync("<a href=\"/SendMultipart\">Send Multipart Request</a><br>");
                await context.Response.WriteAsync("</body></html>");
            });
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetBoundary(string contentType)
        {
            // TODO: Strongly typed headers will take care of this for us
            // TODO: Limit the length of boundary we accept. The spec says ~70 chars.
            var elements = contentType.Split(' ');
            var element = elements.Where(entry => entry.StartsWith("boundary=")).First();
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes
            if (boundary.Length >= 2 && boundary[0] == '"' && boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
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

