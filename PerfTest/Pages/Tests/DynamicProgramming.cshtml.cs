using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfTest.Data;
using PerfTest.Data.Entities;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerfTest.Pages.Tests
{
	public class DynamicProgrammingPageModel : PageModel
	{
		public DynamicProgrammingPageModel(IDbContextFactory<MainDbContext> mainDbFactory) {
			_mainDbFactory = mainDbFactory;
		}

		private readonly IDbContextFactory<MainDbContext> _mainDbFactory;

		public ResultViewModel ResultViewModel { get; private set; } = null!;

		public void OnGetAsync() {
			var timer = Stopwatch.StartNew();
			using var db = _mainDbFactory.CreateDbContext();

			Task.WaitAll(CreateTasks());

			var sku = db.Skus.First();
			DynamicProgrammingSolutionAsync(db, sku, true).GetAwaiter().GetResult();

			ResultViewModel = new ResultViewModel {
				ReadStockCount = sku.Stock,
				ExecutedDuration = timer.Elapsed
			};
		}

		private Task[] CreateTasks() {
			var tasks = new Task[Variables.ExecuteTimes];
			for ( var i = 0; i < tasks.Length; i++ ) {
				tasks[i] = Task.Factory.StartNew(async () => {
					using var db = _mainDbFactory.CreateDbContext();
					var sku = db.Skus.First();
					var changeRecord = new StockChange {
						SkuId = sku.Id,
						ChangeCount = -1
					};
					db.Add(changeRecord);
					await db.SaveChangesAsync();

					await DynamicProgrammingSolutionAsync(db, sku);
				});
			}
			return tasks;
		}

		private Stopwatch Clock => Stopwatch.StartNew();
		private async Task DynamicProgrammingSolutionAsync(MainDbContext db, Sku? sku, bool forceRunning = false) {
			//put a number here just for quick coding
			if ( forceRunning || Clock.ElapsedMilliseconds > 100 ) {
				sku ??= db.Skus.First();
				var laterThanId = sku.LatestSyncedStockChangeId;
				var toId = db.StockChanges.Max(x => x.Id);

				sku.LatestSyncedStockChangeId = toId;
				sku.Stock += db.StockChanges
					.Where(x => x.Id > laterThanId)
					.Where(x => x.Id <= toId)
					.Sum(x => x.ChangeCount);

				await db.SaveChangesAsync();
				Clock.Restart();
			}
		}
	}
}
