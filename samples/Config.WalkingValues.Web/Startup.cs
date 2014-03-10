using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Abstractions;
using Microsoft.AspNet.ConfigurationModel;
using Microsoft.Net.Runtime;

public class Startup
{
    private readonly IApplicationEnvironment _applicationEnvironment;

    public Startup(IApplicationEnvironment applicationEnvironment)
    {
        _applicationEnvironment = applicationEnvironment;
    }

    public void Configuration(IBuilder app)
    {
        // TODO - this should use _applicationEnvironment once it's wired through

        var config = new Configuration();
        config.AddIniFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Config.Sources.ini");
        config.AddEnvironmentVariables();

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";
            await DumpConfig(ctx.Response, config);
        });
    }

    private static async Task DumpConfig(HttpResponse response, IConfiguration config, string indentation = "")
    {
        foreach (var child in config.GetSubKeys())
        {
            await response.WriteAsync(indentation + "[" + child.Key + "] " + config.Get(child.Key) + "\r\n");
            await DumpConfig(response, child.Value, indentation + "  ");
        }
    }
}



namespace Microsoft.Net.Runtime
{
    using System;

    [AssemblyNeutral]
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class AssemblyNeutralAttribute : Attribute
    {
    }
}

namespace Microsoft.Net.Runtime
{
    using System.Runtime.Versioning;
    [AssemblyNeutral]
    public interface IApplicationEnvironment
    {
        string ApplicationName { get; }
        string Version { get; }
        string ApplicationBasePath { get; }
        FrameworkName TargetFramework { get; }
    }
}
