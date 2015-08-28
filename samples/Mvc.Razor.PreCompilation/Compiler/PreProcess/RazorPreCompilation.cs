using System;
using Microsoft.AspNet.Mvc.Razor.Precompilation;

namespace RazorPre
{
    public class MyCompilation : RazorPreCompileModule
    {
        public MyCompilation(IServiceProvider provider) : base(provider)
        {
        }
    }
}