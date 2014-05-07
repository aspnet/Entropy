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
using Microsoft.Data.Entity.SqlServer;

namespace Data.SqlServer
{
    public class Program
    {
        public static async Task Main()
        {
            using (var db = new MyContext())
            {
                if (!await db.Database.ExistsAsync())
                {
                    await db.Database.CreateAsync();
                }
            }

            using (var db = new MyContext())
            {
                // TODO Remove when identity columns work end-to-end
                var nextId = db.Blogs.Any() ? db.Blogs.Max(b => b.BlogId) + 1 : 1;
                await db.AddAsync(new Blog { BlogId = nextId, Name = "Another Blog", Url = "http://example.com" });
                await db.SaveChangesAsync();
           
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
            builder.UseSqlServer(@"Server=(localdb)\v11.0;Database=Blogging;Trusted_Connection=True;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
