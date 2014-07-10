using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;

public class Startup
{
    public void Configure(IBuilder app)
    {
        app.UseOwin(addToPiepline =>
        {
            addToPiepline(next =>
            {
                return Invoke;
            });
        });
    }

    // Invoked once per request.
    public Task Invoke(IDictionary<string, object> environment)
    {
        string responseText = "Hello World";
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

        // See http://owin.org/spec/owin-1.0.0.html for standard environment keys.
        var responseStream = (Stream)environment["owin.ResponseBody"];
        var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];

        responseHeaders["Content-Length"] = new string[] { responseBytes.Length.ToString(CultureInfo.InvariantCulture) };
        responseHeaders["Content-Type"] = new string[] { "text/plain" };

        return responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
