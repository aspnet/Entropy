using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Migrations;
using Microsoft.Data.Relational;
using Microsoft.Data.SqlServer;

namespace Data.SqlServer
{
    public class Program
    {
        public static async Task Main()
        {
            using (var db = new MyContext())
            {
                // TODO Swap to simple top level API when available
                var creator = new SqlServerDataStoreCreator(
                    (SqlServerDataStore)db.Configuration.DataStore,
                    new ModelDiffer(),
                    new SqlServerMigrationOperationSqlGenerator(),
                    new SqlStatementExecutor());

                if(!await creator.ExistsAsync())
                {
                    await creator.CreateAsync(db.Model);
                }
            }

            using (var db = new MyContext())
            {
                // TODO Remove when identity columns work end-to-end
                var nextId = db.Blogs.Any() ? db.Blogs.Max(b => b.BlogId) + 1 : 1;
                db.Add(new Blog { BlogId = nextId, Name = "Another Blog", Url = "http://example.com" });
                await db.SaveChangesAsync();
           
                var blogs = db.Blogs.OrderBy(b => b.Name);
                foreach (var item in blogs)
                {
                    Console.WriteLine(item.Name);
                }
            }
        }
    }

    public class MyContext : EntityContext
    {
        public EntitySet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
            builder
                .WithServices(s => s.AddSqlServer())
                .SqlServerConnectionString(@"Server=(localdb)\v11.0;Database=Blogging;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Blog>()
                .Key(b => b.BlogId);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
