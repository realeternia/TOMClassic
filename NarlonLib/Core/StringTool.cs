using System;

namespace NarlonLib.Core
{
    public class StringTool
    {
        private static readonly Random r = new Random();

		public static string GetRandStringByHex()
		{
            ulong p = (ulong)(r.NextDouble() * ulong.MaxValue);
            return string.Format("{0:X}", p).PadLeft(16, '0');
		}
    }
}
