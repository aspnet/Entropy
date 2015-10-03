using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Owin;
using Microsoft.Extensions.Configuration;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServerFactory : IServerFactory
    {
        private Func<IFeatureCollection, Task> _callback;

        private Task HandleRequest(IDictionary<string, object> env)
        {
            return _callback(new OwinFeatureCollection(env));
        }

        public IFeatureCollection Initialize(IConfiguration configuration)
        {
            // TODO: Parse config
            return new FeatureCollection();
        }

        public IDisposable Start(IFeatureCollection serverFeatures, Func<IFeatureCollection, Task> application)
        {
            var builder = ServerBuilder.New()
                                       .SetAddress(IPAddress.Any)
                                       .SetPort(5000)
                                       .SetOwinApp(OwinWebSocketAcceptAdapter.AdaptWebSockets(HandleRequest));
            _callback = application;
            var server = builder.Build();
            server.Start();
            return server;
        }
    }
}