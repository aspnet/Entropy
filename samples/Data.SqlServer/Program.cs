using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.SqlServer
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new MyContext())
            {
                db.Database.EnsureCreatedAsync().Wait();
            }

            using (var db = new MyContext())
            {
                db.Add(new Blog { Name = "Another Blog", Url = "http://example.com" });
                db.SaveChangesAsync().Wait();

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
