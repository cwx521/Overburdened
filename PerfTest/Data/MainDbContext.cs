#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;
using PerfTest.Data.Entities;

namespace PerfTest.Data
{
	public class MainDbContext : DbContext
	{
		public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) {
		}

		public DbSet<Sku> Skus { get; set; }
		public DbSet<StockChange> StockChanges { get; set; }

		public void TearDown() {
			Database.ExecuteSqlRaw("truncate table Skus");
			Database.ExecuteSqlRaw("truncate table StockChanges");
			Skus.Add(new Sku { Stock = Variables.StartStockCount });
			SaveChanges();
		}
	}
}
