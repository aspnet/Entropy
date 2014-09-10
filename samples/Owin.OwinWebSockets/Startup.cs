using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace SelfHostServer
{
    // http://owin.org/extensions/owin-WebSocket-Extension-v0.4.0.htm
    using WebSocketAccept = Action<IDictionary<string, object>, // options
        Func<IDictionary<string, object>, Task>>; // callback
    using WebSocketCloseAsync =
        Func<int /* closeStatus */,
            string /* closeDescription */,
            CancellationToken /* cancel */,
            Task>;
    using WebSocketReceiveAsync =
        Func<ArraySegment<byte> /* data */,
            CancellationToken /* cancel */,
            Task<Tuple<int /* messageType */,
                bool /* endOfMessage */,
                int /* count */>>>;
    using WebSocketReceiveResult = Tuple<int, // type
        bool, // end of message?
        int>; // count
    using WebSocketSendAsync =
        Func<ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            CancellationToken /* cancel */,
            Task>;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(addToPiepline =>
            {
                addToPiepline(next =>
                {
                    return async env =>
                    {
                        var accept = env["websocket.Accept"] as WebSocketAccept;
                        if (accept == null)
                        {
                            // Not a websocket request
                            await next(env);
                        }
                        else
                        {
                            accept(null, WebSocketEcho);
                        }
                    };
                });
            });

            app.Run(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Not a WebSocket");
            });
        }

        private async Task WebSocketEcho(IDictionary<string, object> websocketContext)
        {
            var sendAsync = (WebSocketSendAsync)websocketContext["websocket.SendAsync"];
            var receiveAsync = (WebSocketReceiveAsync)websocketContext["websocket.ReceiveAsync"];
            var closeAsync = (WebSocketCloseAsync)websocketContext["websocket.CloseAsync"];
            var callCancelled = (CancellationToken)websocketContext["websocket.CallCancelled"];

            byte[] buffer = new byte[1024];
            WebSocketReceiveResult received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);

            object status;
            while (!websocketContext.TryGetValue("websocket.ClientCloseStatus", out status) || (int)status == 0)
            {
                // Echo anything we receive
                await sendAsync(new ArraySegment<byte>(buffer, 0, received.Item3), received.Item1, received.Item2, callCancelled);

                received = await receiveAsync(new ArraySegment<byte>(buffer), callCancelled);
            }

            await closeAsync((int)websocketContext["websocket.ClientCloseStatus"], (string)websocketContext["websocket.ClientCloseDescription"], callCancelled);
        }
    }
}
