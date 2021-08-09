using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfTest.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerfTest.Pages.Tests
{
	public class RawSqlPageModel : PageModel
	{
		public RawSqlPageModel(IDbContextFactory<MainDbContext> mainDbFactory) {
			_mainDbFactory = mainDbFactory;
		}

		private readonly IDbContextFactory<MainDbContext> _mainDbFactory;

		public ResultViewModel ResultViewModel { get; private set; } = null!;

		public void OnGet() {
			var timer = Stopwatch.StartNew();
			using var db = _mainDbFactory.CreateDbContext();

			Task.WaitAll(CreateTasks());

			ResultViewModel = new ResultViewModel {
				ReadStockCount = db.Skus.First().Stock,
				ExecutedDuration = timer.Elapsed
			};
		}

		public Task[] CreateTasks() {
			const string sql = "update Skus set Stock=Stock-1, StockSyncedTime=getdate()";
			var tasks = new Task[Variables.ExecuteTimes];

			for ( var i = 0; i < tasks.Length; i++ ) {
				tasks[i] = Task.Factory.StartNew(async () => {
					using var db = _mainDbFactory.CreateDbContext();
					//db.Database.ExecuteSqlRaw("insert ....");
					await db.Database.ExecuteSqlRawAsync(sql);
				});
			}
			return tasks;
		}
	}
}
