using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PerfTest.Data;

namespace PerfTest
{
	public class TearDownFilter : IPageFilter
	{
		public void OnPageHandlerSelected(PageHandlerSelectedContext context) {
		}

		public void OnPageHandlerExecuted(PageHandlerExecutedContext context) {
		}

		public void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
			using var db = context.HttpContext.RequestServices.GetRequiredService<MainDbContext>();
			db.TearDown();
		}
	}
}
