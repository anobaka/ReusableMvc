using System;
using System.Collections.Generic;
using System.Text;

namespace LazyMortal.ReusableMvc.StaticFiles
{
    /// <inheritdoc />
    /// <summary>
    /// <para>It allows multiple instances on same Action or Controller. </para>
    /// <para>Null or empty array will not block any default static files.</para>
    /// <para>The file types defined on Action will ignore the values on Controller.</para>
    /// </summary>
    public class NoDefaultStaticFilesAttribute : Attribute
    {
        public string[] FileTypes { get; set; }

        public NoDefaultStaticFilesAttribute()
        {
        }
    }
}
