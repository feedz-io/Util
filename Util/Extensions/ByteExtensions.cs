using System;

namespace Feedz.Util.Extensions
{
    public static class ByteExtensions
    {
        public static string ToHexString(this byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", "");
    }
}