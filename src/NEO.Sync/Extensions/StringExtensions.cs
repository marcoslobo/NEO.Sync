using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NEO.Api.Worker
{
    public static class StringExtensions
    {
        public static string Remove0x(this string str)
        {
            return str.Replace("0x", "");
        }
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}
