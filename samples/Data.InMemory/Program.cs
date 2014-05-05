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

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.InMemory;

namespace Data.InMemory
{
    public class Program
    {
        public static async Task Main()
        {
            using (var db = new MyContext())
            {
                db.Add(new Blog {BlogId = 1, Name = "ADO.NET", Url = "http://blogs.msdn.com/adonet"});
                db.Add(new Blog {BlogId = 2, Name = ".NET Framework", Url = "http://blogs.msdn.com/dotnet"});
                db.Add(new Blog {BlogId = 3, Name = "Visual Studio", Url = "http://blogs.msdn.com/visualstudio"});
                await db.SaveChangesAsync();
            }

            using (var db = new MyContext())
            {
                var blogs = db.Blogs.OrderBy(b => b.Name);
                foreach (var item in blogs)
                {
                    Console.WriteLine(item.Name);
                }
            }
        }
    }

    public class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptions builder)
        {
            builder.UseInMemoryStore();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
