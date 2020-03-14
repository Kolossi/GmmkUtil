using System;
using System.Linq;

namespace GmmkUtil
{
    public static class Utils
    {
        // thanks: https://stackoverflow.com/a/321404/2738122
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
