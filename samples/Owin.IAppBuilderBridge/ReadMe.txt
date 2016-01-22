IAppBuilder (Owin.dll) is a component used in Katana v1-3 to build a middleware request pipeline. This interface
has been replaced with IApplicationBuilder (Microsoft.AspNetCore.Http.dll). This sample shows how to use components
dependent on the old interface in the new application pipeline via the OWIN extension point.  See the following
article for additional information: http://blogs.msdn.com/b/webdev/archive/2014/11/14/katana-asp-net-5-and-bridging-the-gap.aspx