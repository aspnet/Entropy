using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Apis;
using MusicStore.Models;

namespace MusicStore.Spa
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                        .AddJsonFile("Config.json")
                        .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(settings =>
            {
                settings.DefaultAdminUsername = Configuration["DefaultAdminUsername"];
                settings.DefaultAdminPassword = Configuration["DefaultAdminPassword"];
            });

            // Add MVC services to the service container
            services.AddMvc();

            // Add EF services to the service container
            services.AddDbContext<MusicStoreContext>(options =>
                {
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
                });

            // Add Identity services to the services container
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<MusicStoreContext>()
                    .AddDefaultTokenProviders();

            // Add application services to the service container
            //services.AddTransient<IModelMetadataProvider, BuddyModelMetadataProvider>();

            // Configure Auth
            services.Configure<AuthorizationOptions>(options =>
            {
                options.AddPolicy("app-ManageStore", new AuthorizationPolicyBuilder().RequireClaim("app-ManageStore", "Allowed").Build());
            });

			Mapper.CreateMap<AlbumChangeDto, Album>();
            Mapper.CreateMap<Album, AlbumChangeDto>();
            Mapper.CreateMap<Album, AlbumResultDto>();
            Mapper.CreateMap<AlbumResultDto, Album>();
            Mapper.CreateMap<Artist, ArtistResultDto>();
            Mapper.CreateMap<ArtistResultDto, Artist>();
            Mapper.CreateMap<Genre, GenreResultDto>();
            Mapper.CreateMap<GenreResultDto, Genre>();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Initialize the sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();

            // Configure the HTTP request pipeline

            // Add cookie auth
            app.UseIdentity();

            // Add static files
            app.UseStaticFiles();

            // Add MVC
            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}

