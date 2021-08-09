using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PerfTest.Data;
using System.Linq;

namespace PerfTest
{
	public class Startup
	{
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services) {
			services.AddMemoryCache();
			services.AddRazorPages().AddMvcOptions(x => {
				x.Filters.Add<TearDownFilter>();
			});

			var connstr = Configuration.GetConnectionString("SqlDev");
			services.AddDbContextPool<MainDbContext>(x => x.UseSqlServer(connstr));
			services.AddPooledDbContextFactory<MainDbContext>(x => x.UseSqlServer(connstr));
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseDeveloperExceptionPage();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseEndpoints(endpoints => {
				endpoints.MapRazorPages();
			});

			using var scope = app.ApplicationServices.CreateScope();
			using var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();
			if ( db.Database.GetPendingMigrations().Any() ) {
				db.Database.EnsureDeleted();
				db.Database.Migrate();
			}
		}
	}
}
