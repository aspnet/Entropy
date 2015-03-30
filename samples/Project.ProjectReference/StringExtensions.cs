using System;

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
