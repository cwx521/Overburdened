using System;

namespace PerfTest
{
	public class ResultViewModel
	{
		public int ExecutedTimes => Variables.ExecuteTimes;
		public int StartStockCount => Variables.StartStockCount;
		public int ReadStockCount { get; set; }
		public TimeSpan ExecutedDuration { get; set; }
		public bool IsCorrectResult => ReadStockCount == StartStockCount - ExecutedTimes;
	}
}
