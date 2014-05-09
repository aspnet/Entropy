using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ProjectReference
{
    public static class StringExtensions
    {
        public static string ToLower2(this string s)
        {
            return s.ToLowerInvariant();
        }
    }
}
