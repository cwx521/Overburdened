using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfTest.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerfTest.Pages.Tests
{
	public class CrashPageModel : PageModel
	{
		public CrashPageModel(MainDbContext dbContext) {
			_db = dbContext;
		}

		private readonly MainDbContext _db;

		public ResultViewModel ResultViewModel { get; private set; } = null!;

		public void OnGet() {
			var timer = Stopwatch.StartNew();
			Task.WaitAll(CreateTasks());

			ResultViewModel = new ResultViewModel {
				ReadStockCount = _db.Skus.First().Stock,
				ExecutedDuration = timer.Elapsed
			};
		}

		public Task[] CreateTasks() {
			const string sql = "update Skus set Stock=Stock-1, StockSyncedTime=getdate()";
			var tasks = new Task[Variables.ExecuteTimes];

			for ( var i = 0; i < tasks.Length; i++ ) {
				tasks[i] = Task.Factory.StartNew(async () => {
					//db.Database.ExecuteSqlRaw("insert ....");
					await _db.Database.ExecuteSqlRawAsync(sql);
				});
			}
			return tasks;
		}
	}
}
