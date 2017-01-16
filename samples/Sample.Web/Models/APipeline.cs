using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.ReusableMvc.Pipelines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Sample.Web.Models
{
	public class APipeline : ReusablePipeline
	{
		public override string Name { get; set; } = "A";

		public override Task<bool> ResolveAsync(HttpContext ctx)
		{
			return
				Task.FromResult(ctx.Request.Path.Value.StartsWith($"/{Name}/", StringComparison.OrdinalIgnoreCase) ||
				                ctx.Request.Path.Value.Equals($"/{Name}", StringComparison.OrdinalIgnoreCase));
		}

		public override Task ConfigurePipeline(IApplicationBuilder app)
		{
			return TaskCache.CompletedTask;
		}
	}
}