using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Owin;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServer : IServer
    {
        private Func<IFeatureCollection, Task> _callback;
        private INowinServer _nowinServer;

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public NowinServer()
        {
            Features.Set<IServerAddressesFeature>(new ServerAddressesFeature());
        }

        public void Start<TContext>(IHttpApplication<TContext> application)
        {
            // Note that this example does not take into account of Nowin's "server.OnSendingHeaders" callback.
            // Ideally we should ensure this method is fired before disposing the context.
            _callback = async features =>
            {
                var context = application.CreateContext(features);
                try
                {
                    await application.ProcessRequestAsync(context);
                }
                catch (Exception ex)
                {
                    application.DisposeContext(context, ex);
                    throw;
                }
                application.DisposeContext(context, null);
            };

            var address = Features.Get<IServerAddressesFeature>().Addresses.First();
            var port = new Uri(address).Port;
            var serverBuilder = ServerBuilder.New().SetAddress(IPAddress.Loopback).SetPort(port);

            _nowinServer = serverBuilder.SetOwinApp(OwinWebSocketAcceptAdapter.AdaptWebSockets(HandleRequest)).Build();
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
            return _callback(new FeatureCollection(new OwinFeatureCollection(env)));
        }
    }
}
