#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using System;
using System.ComponentModel.DataAnnotations;

namespace PerfTest.Data.Entities
{
	//Stock Keeping Unit
	public class Sku
	{
		[Key]
		public int Id { get; set; }

		public int Stock { get; set; }

		public int LatestSyncedStockChangeId { get; set; }

		public DateTime StockSyncedTime { get; set; } = DateTime.Now;
	}
}
