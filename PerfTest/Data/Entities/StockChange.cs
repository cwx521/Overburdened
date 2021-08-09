using System;
using System.ComponentModel.DataAnnotations;

namespace PerfTest.Data.Entities
{
	public class StockChange
	{
		[Key]
		public int Id { get; set; }

		public int SkuId { get; set; }

		public int? AssociatedOrderId { get; set; }

		public int ChangeCount { get; set; }

		public DateTime ChangedTime { get; set; }
	}
}