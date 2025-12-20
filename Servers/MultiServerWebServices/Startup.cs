using Alcatraz.Context;
using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.Extension.LinqSQL;
using WebAPIService.LeaderboardService;

namespace MultiServerWebServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var secOpts = Configuration.GetSection("Services").Get<MConfiguration>();

            services.AddDbContext<MainDbContext>(opt =>
            {
                MainDbContext.OnContextBuilding(opt, (DBType)secOpts!.DbType, secOpts.QuazalDbConnectionString);
            });
            services.AddDbContext<LeaderboardDbContext>(opt =>
            {
                LeaderboardDbContext.OnContextBuilding(opt, (DBType)secOpts!.DbType, secOpts.WebAPILeaderboardDbConnectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MainDbContext dbContext)
        {
            // update database if haven't
            dbContext.Database.Migrate();

            // if db context was used during migrations, send changes after all migrations done
            dbContext.SaveChanges();
        }
    }
}