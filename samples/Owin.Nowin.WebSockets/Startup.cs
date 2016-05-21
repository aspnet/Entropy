using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NowinWebSockets
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine("It's a websocket!");
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("Accepted websocket.");
                    await EchoWebSocket(webSocket);
                    Console.WriteLine("Socket closed.");
                }
                else
                {
                    await next();
                }
            });

            app.Run(context =>
            {
                return context.Response.WriteAsync("Hello World");
            });
        }

        private async Task EchoWebSocket(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!webSocket.CloseStatus.HasValue)
            {
                // Echo anything we receive
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, received.Count), received.MessageType, received.EndOfMessage, CancellationToken.None);

                received = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseNowin()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
