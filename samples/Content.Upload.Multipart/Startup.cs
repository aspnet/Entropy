using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.HttpFeature;

namespace Content.Upload.Multipart
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (!context.Request.HasMultipartContentType())
                {
                    await next();
                    return;
                }

                var parts = await context.Request.ReadMultipartBodyAsync();

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("<html><body>Multipart: received " + parts.Count() + " parts.<br>");
                foreach (var part in parts)
                {
                    await context.Response.WriteAsync("- Header count: " + part.Headers.Count + ", Body length: " + part.Body.Length + "<br>");
                    foreach (var headerPair in part.Headers)
                    {
                        await context.Response.WriteAsync("-- " + headerPair.Key + ": " + string.Join(", ", headerPair.Value) + "<br>");
                    }

                    // Nested?
                    if (part.HasMultipartContentType())
                    {
                        var subParts = await part.ReadMultipartBodyAsync();
                        await context.Response.WriteAsync("-- Nested Multipart: received " + subParts.Count() + " parts.<br>");

                        foreach (var subPart in subParts)
                        {
                            await context.Response.WriteAsync("--- Header count: " + subPart.Headers.Count + ", Body length: " + subPart.Body.Length + "<br>");
                            foreach (var headerPair in subPart.Headers)
                            {
                                await context.Response.WriteAsync("---- " + headerPair.Key + ": " + string.Join(", ", headerPair.Value) + "<br>");
                            }
                        }
                    }
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
                client.BaseAddress = new Uri("http://localhost:" + context.GetFeature<IHttpConnectionFeature>().LocalPort);

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
    }
}
