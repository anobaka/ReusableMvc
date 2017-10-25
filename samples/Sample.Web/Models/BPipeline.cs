using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LazyMortal.ReusableMvc.Options;
using LazyMortal.ReusableMvc.Pipelines;

namespace Sample.Web.Models
{
    public class BPipeline : APipeline
    {
        public BPipeline(ReusablePipelineOptions options) : base(options)
        {
        }
    }
}