// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using Microsoft.AspNet.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Builder.Filtering.Web
{
    public class Startup
    {
        public void Configuration(IBuilder app)
        {
            app.Use(next => context => FilterAsync(context, next));

            app.Run(HelloWorldAsync);
        }

        public async Task FilterAsync(HttpContext context, RequestDelegate next)
        {
            var body = context.Response.Body;
            var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                context.Response.Headers["CustomHeader"] = "My Header";

                await context.Response.WriteAsync("Before\r\n");
                await next(context);
                await context.Response.WriteAsync("After\r\n");

                buffer.Position = 0;
                await buffer.CopyToAsync(body);
            }
            finally
            {
                context.Response.Body = body;
            }
        }

        public async Task HelloWorldAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Hello world\r\n");
        }
    }
}
