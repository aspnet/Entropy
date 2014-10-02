using System;
using Microsoft.AspNet.Mvc;

namespace RazorPre
{
    public class MyCompilation : RazorPreCompileModule
    {
        public MyCompilation(IServiceProvider provider) : base(provider)
        {
        }
    }
}