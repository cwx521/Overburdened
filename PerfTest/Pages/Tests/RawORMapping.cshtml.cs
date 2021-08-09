using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfTest.Data;
using PerfTest.Data.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerfTest.Pages.Tests
{
	public class RawORMappingPageModel : PageModel
	{
		public RawORMappingPageModel(IDbContextFactory<MainDbContext> mainDbFactory) {
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
			var tasks = new Task[Variables.ExecuteTimes];
			for ( var i = 0; i < tasks.Length; i++ ) {
				tasks[i] = Task.Factory.StartNew(async () => {
					using var db = _mainDbFactory.CreateDbContext();

					var sku = db.Skus.First();
					sku.Stock--;
					sku.StockSyncedTime = DateTime.Now;

					var changeRecord = new StockChange {
						SkuId = sku.Id,
						ChangeCount = -1
					};
					db.Add(changeRecord);

					await db.SaveChangesAsync();
				});
			}
			return tasks;
		}
	}
}
