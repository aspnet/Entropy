using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting.Server;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Owin;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServer : IServer
    {
        private Func<IFeatureCollection, Task> _callback;
        private INowinServer _nowinServer;
        private ServerBuilder _serverBuilder;

        IFeatureCollection IServer.Features { get; } = new FeatureCollection();

        public NowinServer(ServerBuilder serverBuilder)
        {
            if (serverBuilder == null)
            {
                throw new ArgumentNullException(nameof(serverBuilder));
            }
            _serverBuilder = serverBuilder;
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
            return _callback(new FeatureCollection(new OwinFeatureCollection(env)));
        }
    }
}
