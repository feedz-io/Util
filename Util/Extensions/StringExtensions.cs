using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Feedz.Util.Extensions
{
    public static class StringExtensions
    {
        public static string Join<T>(this IEnumerable<T> values, string seperator)
            => values == null ? null : string.Join(seperator, values);

        public static T[] Split<T>(this string value, char seperator, Func<string, T> select)
            => value?.Split(seperator).Select(select).ToArray();

        public static string Md5(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using (var md5 = MD5.Create())
                return md5.ComputeHash(bytes).ToHexString().ToLower();
        }
    }
}