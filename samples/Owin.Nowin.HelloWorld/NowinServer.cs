using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Options;

namespace Nowin
{
    public class NowinServer : IServer
    {
        private INowinServer _nowinServer;
        private ServerBuilder _builder;

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public NowinServer(IOptions<ServerBuilder> options)
        {
            Features.Set<IServerAddressesFeature>(new ServerAddressesFeature());
            _builder = options.Value;
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            // Note that this example does not take into account of Nowin's "server.OnSendingHeaders" callback.
            // Ideally we should ensure this method is fired before disposing the context. 
            Func<IDictionary<string, object>, Task> appFunc = async env =>
            {
                // The reason for 2 level of wrapping is because the OwinFeatureCollection isn't mutable
                // so features can't be added
                var features = new FeatureCollection(new OwinFeatureCollection(env));

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

            // Add the web socket adapter so we can turn OWIN websockets into ASP.NET Core compatible web sockets.
            // The calling pattern is a bit different
            appFunc = OwinWebSocketAcceptAdapter.AdaptWebSockets(appFunc);

            // Get the server addresses
            var address = Features.Get<IServerAddressesFeature>().Addresses.First();

            var uri = new Uri(address);
            var port = uri.Port;
            IPAddress ip;
            if (!IPAddress.TryParse(uri.Host, out ip))
            {
                ip = IPAddress.Loopback;
            }

            _nowinServer = _builder.SetAddress(ip)
                                    .SetPort(port)
                                    .SetOwinApp(appFunc)
                                    .Build();
            _nowinServer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _nowinServer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
