using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServerFactory : IServerFactory
    {
        public IServer CreateServer(IConfiguration configuration)
        {
            return new NowinServer(ServerBuilder.New().SetAddress(IPAddress.Any).SetPort(5000));
        }
    }
}