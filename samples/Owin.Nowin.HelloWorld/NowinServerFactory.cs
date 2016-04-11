using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Nowin;

namespace NowinWebSockets
{
    public class NowinServerFactory : IServerFactory
    {
        public IServer CreateServer(IConfiguration configuration)
        {
            var port = configuration["PORT"];
            return new NowinServer(ServerBuilder.New().SetAddress(IPAddress.Any).SetPort(string.IsNullOrEmpty(port) ? 5000 : int.Parse(port)));
        }
    }
}