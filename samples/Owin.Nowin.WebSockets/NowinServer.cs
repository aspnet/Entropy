using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Owin;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServer : IServer
    {
        private RequestDelegate _callback;
        private INowinServer _nowinServer;
        private ServerBuilder _serverBuilder;

        IFeatureCollection IServer.Features { get; }

        public NowinServer(ServerBuilder serverBuilder)
        {
            if (serverBuilder == null)
            {
                throw new ArgumentNullException(nameof(serverBuilder));
            }
            _serverBuilder = serverBuilder;
        }

        public void Start(RequestDelegate requestDelegate)
        {
            _callback = requestDelegate;
            _nowinServer = _serverBuilder.SetOwinApp(OwinWebSocketAcceptAdapter.AdaptWebSockets(HandleRequest)).Build();
            _nowinServer.Start();
        }

        public void Dispose()
        {
            if (_nowinServer != null)
            {
                _nowinServer.Dispose();
            }
        }

        private Task HandleRequest(IDictionary<string, object> env)
        {
            return _callback(new DefaultHttpContext(new OwinFeatureCollection(env)));
        }
    }
}
